using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EncountersController : BaseEntityController<Encounter>
    {
        public EncountersController(DMContext context,
            ILogger<EncountersController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
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
        public async Task<IActionResult> CreateNewEncounter([FromBody] Encounter request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException("Request cannot be null!");

                if (ModelState.IsValid)
                {
                    var entity = await CreateNewEncounterInContext(request);
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

        protected async Task<Encounter> CreateNewEncounterInContext(Encounter request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            request.User = currentUser;
            foreach (var data in request.InitativeData)
            {
                if (data.Character != null)
                    data.Character = _context.Characters.FirstOrDefault(x => x.Id == data.Character.Id);
                if (data.Creature != null)
                    data.Creature = _context.Creatures.FirstOrDefault(x => x.Id == data.Creature.Id);
            }
            var entry = _context.Update(request);
            return entry.Entity;
        }



        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEncounterById(Guid id, [FromBody] Encounter request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.Encounters
                    .Include(e => e.InitativeData)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateEncounter(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Encounter> UpdateEncounter(Encounter encounterFromRepo, Encounter request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            foreach (var data in request.InitativeData)
            {
                if (data.Being != null)
                {
                    data.Being.User = null;
                    data.Being.ForcePowers = null;
                    data.Being.TechPowers = null;
                }
            }

            var entity = _context.Entry(encounterFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;

            List<InitativeData> initativeData = new();
            foreach (var data in request.InitativeData)
            {
                var dataFromRepo = _context.InitativeData.FirstOrDefault(x => x.Id == data.Id);
                EntityEntry<InitativeData> initativeEntry;
                if (dataFromRepo == null)
                    initativeEntry = _context.InitativeData.Add(new InitativeData());
                else
                    initativeEntry = _context.Entry(dataFromRepo);
                data.Id = initativeEntry.Entity.Id;
                initativeEntry.CurrentValues.SetValues(data);
                initativeEntry.Entity.User = currentUser;

                initativeData.Add(initativeEntry.Entity);
            }
            entity.Entity.InitativeData = initativeData;
            return entity.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEncounterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

