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
    public class CreaturesController : BaseEntityController<Creature>
    {
        public CreaturesController(DMContext context,
            ILogger<CreaturesController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
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
        public async Task<IActionResult> CreateNewCreature([FromBody] Creature request)
        {
            return await CreateNewEntity(request);
        }
       

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCreatureById(Guid id, [FromBody] Creature request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.GetQueryable<Creature>()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateCreature(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Creature> UpdateCreature(Creature creatureFromRepo, Creature request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entity = _context.Entry(creatureFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;
            var forcePowers = _context.ForcePowers
                .Where(x => request.ForcePowers.Select(f => f.Id).Contains(x.Id)).ToList();
            entity.Entity.ForcePowers = forcePowers;
            var techPowers = _context.TechPowers
                .Where(x => request.TechPowers.Select(f => f.Id).Contains(x.Id)).ToList();
            entity.Entity.TechPowers = techPowers;
            var actions = new List<BaseAction>();
            foreach (var action in request.Actions)
            {
                var actionFromRepo = _context.Actions.FirstOrDefault(x => x.Id == action.Id);
                EntityEntry<BaseAction> actionEntry;
                if (actionFromRepo == null)
                {
                    actionEntry = _context.Add(new BaseAction());
                    actionEntry.Entity.Id = Guid.NewGuid();
                    action.Id = actionEntry.Entity.Id;
                }
                else
                    actionEntry = _context.Entry(actionFromRepo);
                actionEntry.CurrentValues.SetValues(action);
                actions.Add(actionEntry.Entity);
            }

            entity.Entity.Actions = actions;
            return entity.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCreatureById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

