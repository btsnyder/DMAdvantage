using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DMAdvantage.Server.Controllers
{
    public class BaseEntityController<TEntity> : Controller where TEntity : BaseEntity, new()
    {
        protected readonly DMContext _context;
        protected readonly ILogger<BaseEntityController<TEntity>> _logger;
        protected readonly UserManager<User> _userManager;
        protected string apiPath { get; set; }

        public BaseEntityController(DMContext context,
            ILogger<BaseEntityController<TEntity>> logger,
            UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            apiPath = typeof(TEntity).Name.ToLower() + "s";
        }

        public IActionResult GetAllEntities(PagingParameters? paging = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                DbSet<TEntity> dbSet = _context.Set<TEntity>();
                var entities = dbSet.Where(c => c.User != null && c.User.UserName == username).AsNoTracking().OrderBy(c => c.OrderBy());

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

        public IActionResult GetAllEntities<T>(NamedSearchParameters<T> searching, PagingParameters? paging = null) where T : BaseEntity
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching.Search;

                var dbSet = _context.Set<T>();
                IQueryable<T> query;
                if (search == null)
                    query = dbSet
                        .Where(c => c.User != null && c.User.UserName == username)
                        .AsNoTracking();
                else
                    query = dbSet
                        .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search.ToLower()))
                        .AsNoTracking();

                var entities = query.ToList().OrderBy(c => c.OrderBy());

                if (paging == null)
                    return Ok(entities);

                var pagedResults = PagedList<T>.ToPagedList(entities, paging);
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

                DbSet<TEntity> dbSet = _context.Set<TEntity>();
                var entity = dbSet.AsNoTracking()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

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
                        return Created($"/api/{apiPath}/{entity.Id}", entity);
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
                    return Created($"/api/{apiPath}/{entity.Id}", entity);
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
