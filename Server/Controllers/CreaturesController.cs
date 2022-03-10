using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CreaturesController : BaseEntityController<Creature, CreatureResponse, CreatureRequest>
    {
        public CreaturesController(IRepository repository,
            ILogger<CreaturesController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCreatures([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Creature>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetCreatureById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCreature([FromBody] CreatureRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var creature = await CreateNewCreatureInContext(request);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/creatures/{creature.Id}", _mapper.Map<CreatureResponse>(creature));
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

        private async Task<Creature> CreateNewCreatureInContext(CreatureRequest request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entry = _repository.Context.Add(new Creature());
            entry.Entity.User = currentUser;
            entry.CurrentValues.SetValues(request);
            entry.Entity.ActionsCache = JsonSerializer.Serialize(request.Actions);
            return entry.Entity;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCreatureById(Guid id, [FromBody] CreatureRequest request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _repository.Context.Creatures
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    var creature = await CreateNewCreatureInContext(request);

                    return Created($"/api/creatures/{creature.Id}", _mapper.Map<CreatureResponse>(creature));
                }

                var entry = _repository.Context.Entry(entityFromRepo);
                entry.CurrentValues.SetValues(request);
                entry.Entity.ActionsCache = JsonSerializer.Serialize(request.Actions);
                _repository.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCreatureById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

