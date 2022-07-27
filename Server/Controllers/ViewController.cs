using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    public class ViewController : Controller
    {
        private readonly DMContext _context;
        private readonly ILogger<ViewController> _logger;

        public ViewController(DMContext context,
            ILogger<ViewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("encounter/{id:guid}")]
        public async Task<IActionResult> GetEncounterById(Guid id)
        {
            try
            {
                var entity = await _context.Encounters
                    .Include(e => e.InitativeData).ThenInclude(i => i.EquipmentQuantities)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
 
                if (entity != null)
                {
                    await SetInitativeDataForeignKeys(entity.InitativeData);
                    return Ok(entity);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return encounter: {ex}");
                return BadRequest("Failed to return encounter");
            }
        }

        private async Task SetInitativeDataForeignKeys(ICollection<InitativeData> data)
        {
            foreach (var i in data)
            {
                if (i.CharacterId != null)
                {
                    i.Character = await _context.GetQueryable<Character>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == i.CharacterId);
                }
                if (i.CreatureId != null)
                {
                    i.Creature = await _context.GetQueryable<Creature>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == i.CreatureId);
                }
                foreach (var e in i.EquipmentQuantities)
                {
                    e.Equipment = await _context.Equipments.FirstOrDefaultAsync(x => x.Id == e.EquipmentId);
                }
            }
        }

        [HttpGet("shipencounter/{id:guid}")]
        public async Task<IActionResult> GetShipEncounterById(Guid id)
        {
            try
            {
                var entity = await _context.ShipEncounters
                     .Include(i => i.InitativeData)
                     .AsNoTracking()
                     .FirstOrDefaultAsync(x => x.Id == id);

                if (entity != null)
                {
                    await SetShipInitativeDataForeignKeys(entity.InitativeData);
                    return Ok(entity);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return encounter: {ex}");
                return BadRequest("Failed to return encounter");
            }
        }

        private async Task SetShipInitativeDataForeignKeys(ICollection<ShipInitativeData> data)
        {
            foreach (var i in data)
            {
                if (i.PlayerShipId != null)
                {
                    i.PlayerShip = await _context.GetQueryable<PlayerShip>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == i.PlayerShipId);
                }
                if (i.EnemyShipId != null)
                {
                    i.EnemyShip = await _context.GetQueryable<EnemyShip>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == i.EnemyShipId);
                }
            }
        }

        [HttpGet("characters")]
        public IActionResult GetCharactersByIds([FromQuery] Guid[] ids)
        {
            return GetEntitiesByIds<Character>(ids);
        }

        [HttpGet("characters/player/{name}")]
        public IActionResult GetCharacterByPlayerName([FromRoute] string name)
        {
            try
            {
                var result = _context.GetQueryable<Character>()
                    .AsNoTracking()
                    .FirstOrDefault(c => c.PlayerName == name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return character by player name: {ex}");
                return BadRequest("Failed to return character by player name");
            }
        }

        [HttpGet("creatures")]
        public IActionResult GetCreaturesByIds([FromQuery] Guid[] ids)
        {
            return GetEntitiesByIds<Creature>(ids);
        }

        [HttpGet("forcepowers")]
        public IActionResult GetForcePowersByIds([FromQuery] Guid[] ids)
        {
            return GetEntitiesByIds<ForcePower>(ids);
        }

        [HttpGet("abilitiesids")]
        public IActionResult GetAbilitiesByIds([FromQuery] Guid[] ids)
        {
            return GetEntitiesByIds<Ability>(ids);
        }

        private IActionResult GetEntitiesByIds<T>(Guid[] ids) where T : BaseEntity
        {
            try
            {
                if (ids.Any())
                    return Ok(_context.GetQueryable<T>().AsNoTracking().Where(x => ids.Contains(x.Id)));
                return Ok(_context.Abilities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }
    }
}
