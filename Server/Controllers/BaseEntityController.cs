using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMAdvantage.Server.Controllers
{
    public class BaseEntityController<TEntity, TResponse, TRequest> : Controller where TEntity : BaseEntity
    {
        private readonly IRepository _repository;
        private readonly ILogger<BaseEntityController<TEntity, TResponse, TRequest>> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public BaseEntityController(IRepository repository,
            ILogger<BaseEntityController<TEntity, TResponse, TRequest>> logger,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        protected IActionResult GetAllEntities()
        {
            try
            {
                var username = User?.Identity?.Name ?? string.Empty;

                var results = _repository.GetAllEntities<TEntity>(username);
                return Ok(_mapper.Map<IEnumerable<TResponse>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest($"Failed to return entities");
            }
        }

        protected IActionResult GetEntityById(Guid id)
        {
            try
            {

                var username = User?.Identity?.Name ?? string.Empty;

                var entity = _repository.GetEntityById<TEntity>(id, username);
                if (entity != null)
                    return Ok(_mapper.Map<TResponse>(entity));
                else return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entity: {ex}");
                return BadRequest($"Failed to return entity");
            }
        }

        protected async Task<IActionResult> CreateNewEntity([FromBody] TRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newEntity = _mapper.Map<TEntity>(request);

                    var currentUser = await _userManager.FindByNameAsync(User?.Identity?.Name);
                    newEntity.User = currentUser;

                    _repository.AddEntity(newEntity);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/{typeof(TEntity).Name.ToLower()}s/{newEntity.Id}", _mapper.Map<TResponse>(newEntity));
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

        protected async Task<IActionResult> UpdateEntityById(Guid id, [FromBody] TRequest request)
        {
            try
            {
                var username = User?.Identity?.Name ?? string.Empty;

                var entityFromRepo = _repository.GetEntityById<TEntity>(id, username);

                if (entityFromRepo == null)
                {
                    var entityToAdd = _mapper.Map<TEntity>(request);

                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    entityToAdd.User = currentUser;

                    _repository.AddEntity(entityToAdd);
                    _repository.SaveAll();

                    return Created($"/api/{typeof(TEntity).Name.ToLower()}s/{entityToAdd.Id}", _mapper.Map<TResponse>(entityToAdd));
                }

                _mapper.Map(request, entityFromRepo);
                _repository.SaveAll();
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
                var username = User?.Identity?.Name ?? string.Empty;

                var entityFromRepo = _repository.GetEntityById<TEntity>(id, username);

                if (entityFromRepo == null)
                {
                    return NotFound();
                }

                _repository.RemoveEntity(entityFromRepo);
                _repository.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete entity: {ex}");
            }

            return BadRequest("Failed to delete entity.");
        }
    }
}
