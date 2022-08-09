using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAdvantage.Server.Controllers
{
    public class BaseEntityController<TEntity> : Controller where TEntity : BaseEntity, new()
    {
        protected readonly DMContext _context;
        protected readonly ILogger<BaseEntityController<TEntity>> _logger;
        protected readonly UserManager<User> _userManager;

        public BaseEntityController(DMContext context,
            ILogger<BaseEntityController<TEntity>> logger,
            UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult GetAllEntities(PagingParameters? paging = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                IQueryable<TEntity> queryable = _context.GetQueryable<TEntity>();
                var query = queryable.AsNoTrackingWithUser(username);
                var entities = query.OrderBy(c => c.ToString());

                if (paging == null)
                    return Ok(entities);
                
                var pagedResults = PagedList<TEntity>.ToPagedList(entities.ToList(), paging);
                Response.SetPagedHeader(pagedResults);
                return Ok(pagedResults);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        public IActionResult GetAllEntities(NamedSearchParameters<TEntity> searching, PagingParameters? paging = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching.Search;
                var query = _context.GetQueryable<TEntity>(false).AsNoTrackingWithUser(username);
                if (search != null)
                    query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(search.ToLower()));

                var entities = query.ToList().OrderBy(c => c.ToString());

                if (paging == null)
                    return Ok(entities);

                var pagedResults = PagedList<TEntity>.ToPagedList(entities, paging);
                Response.SetPagedHeader(pagedResults);
                return Ok(pagedResults);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        protected IActionResult GetEntityById(Guid id)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                var entity = _context.GetQueryable<TEntity>().GetEntityByIdAndUser(username, id, false);

                if (entity == null) return NotFound();
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entity: {ex}");
                return BadRequest("Failed to return entity");
            }
        }

        protected async Task<IActionResult> CreateNewEntity([FromBody] TEntity request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException("Request cannot be null!");

                if (ModelState.IsValid)
                {
                    var entity = await CreateNewEntityInContext(request);
                    if (_context.SaveAll())
                    {
                        return Created($"/api/{GenericHelpers.GetPath<TEntity>()}/{entity.Id}", entity);
                    }
                }
                else
                {
                    _logger.LogError($"Invalid ModelState: {ModelState}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save new entity: {ex}");
            }

            return BadRequest("Failed to save new entity.");
        }

        protected async Task<IActionResult> UpdateEntityById(Guid id, [FromBody] TEntity request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");
                if (request == null)
                    throw new ArgumentNullException("Request cannot be null!");

                DbSet<TEntity> dbSet = _context.Set<TEntity>();
                var entityFromRepo = dbSet.AsNoTracking()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                request.Id = id;
                if (entityFromRepo == null)
                {
                    var entity = await CreateNewEntityInContext(request);
                    return Created($"/api/{GenericHelpers.GetPath<TEntity>()}/{entity.Id}", entity);
                }

                await CreateNewEntityInContext(request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        protected IActionResult DeleteEntityById(Guid id)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                DbSet<TEntity> dbSet = _context.Set<TEntity>();
                var entityFromRepo = dbSet.FirstOrDefault(x => x.Id == id && x.User != null && x.User.UserName == username);

                if (entityFromRepo == null)
                {
                    return NotFound();
                }

                _context.Remove(entityFromRepo);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete entity: {ex}");
            }

            return BadRequest("Failed to delete entity.");
        }

        protected async Task<TEntity> CreateNewEntityInContext(TEntity request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            request.User = currentUser;
            var entry = _context.Update(request);
            return entry.Entity;
        }
    }
}
