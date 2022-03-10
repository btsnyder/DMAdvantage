using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EncountersController : BaseEntityController<Encounter, EncounterResponse, EncounterRequest>
    {
        public EncountersController(IRepository repository,
            ILogger<EncountersController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllEncounters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Encounter>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetEncounterById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEncounter([FromBody] EncounterRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var character = await CreateNewEncounterInContext(request);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/encounters/{character.Id}", _mapper.Map<EncounterResponse>(character));
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEncounterById(Guid id, [FromBody] EncounterRequest request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _repository.Context.Encounters
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    var encounter = await CreateNewEncounterInContext(request);

                    return Created($"/api/encounters/{encounter.Id}", _mapper.Map<EncounterResponse>(encounter));
                }

                await CreateNewEncounterInContext(request, entityFromRepo);
                _repository.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Encounter> CreateNewEncounterInContext(EncounterRequest request, Encounter? encounter = null)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            EntityEntry<Encounter> entry;
            if (encounter == null)
                entry = _repository.Context.Add(new Encounter());
            else
                entry = _repository.Context.Entry(encounter);
            entry.Entity.User = currentUser;
            entry.CurrentValues.SetValues(request);
            entry.Entity.DataCache = JsonSerializer.Serialize(request.Data);
            entry.Entity.ConcentrationCache = JsonSerializer.Serialize(request.ConcentrationPowers);
            return entry.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEncounterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

