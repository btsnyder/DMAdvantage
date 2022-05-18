using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using DMAdvantage.Shared.Services.Kafka;
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
    public class ShipEncountersController : BaseEntityController<ShipEncounter>
    {
        private readonly KafkaProducer _producer;

        public ShipEncountersController(DMContext context,
            ILogger<ShipEncountersController> logger,
            UserManager<User> userManager,
            KafkaProducer producer)
            : base(context, logger, userManager)
        {
            _producer = producer;
        }

        [HttpGet]
        public IActionResult GetAllEncounters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<ShipEncounter>? searching = null)
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
        public async Task<IActionResult> CreateNewEncounter([FromBody] ShipEncounter request)
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
                        return Created($"/api/{DMTypeExtensions.GetPath<ShipEncounter>()}/{entity.Id}", entity);
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

        protected async Task<ShipEncounter> CreateNewEncounterInContext(ShipEncounter request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            request.User = currentUser;
            foreach (var data in request.InitativeData)
            {
                if (data.PlayerShip != null)
                    data.PlayerShip = _context.PlayerShips.FirstOrDefault(x => x.Id == data.PlayerShip.Id);
                if (data.EnemyShip != null)
                    data.EnemyShip = _context.EnemyShips.FirstOrDefault(x => x.Id == data.EnemyShip.Id);
            }
            var entry = _context.Update(request);
            return entry.Entity;
        }



        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEncounterById(Guid id, [FromBody] ShipEncounter request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.ShipEncounters
                    .Include(e => e.InitativeData)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateEncounter(entityFromRepo, request);
                _context.SaveAll();
                _producer.SendMessage(new KafkaMessage { Topic = Topics.ShipEncounters, User = username, Value = KafkaValues.Updated });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<ShipEncounter> UpdateEncounter(ShipEncounter encounterFromRepo, ShipEncounter request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            foreach (var data in request.InitativeData)
            {
                if (data.Ship != null)
                {
                    data.Ship.User = null;
                }
            }

            var entity = _context.Entry(encounterFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;

            List<ShipInitativeData> initativeData = new();
            foreach (var data in request.InitativeData)
            {
                var dataFromRepo = _context.ShipInitativeData.FirstOrDefault(x => x.Id == data.Id);
                EntityEntry<ShipInitativeData> initativeEntry;
                if (dataFromRepo == null)
                    initativeEntry = _context.ShipInitativeData.Add(new ShipInitativeData());
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

