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
                    .Include(e => e.InitativeData)
                    .FirstOrDefaultAsync(x => x.Id == id);
 
                if (entity != null)
                {
                    foreach (var i in entity.InitativeData)
                    {
                        _context.Entry(i).Reference(i => i.Character).Load();
                        if (i.Character != null)
                        {
                            var character = _context.Entry(i.Character);
                            character.Collection(c => c.Abilities).Load();
                            character.Reference(c => c.Class).Load();
                            character.Collection(c => c.ForcePowers).Load();
                            character.Collection(c => c.Weapons).Load();
                            foreach (var weapon in character.Entity.Weapons)
                            {
                                _context.Entry(weapon).Collection(w => w.Properties).Load();
                            }
                        }
                        if (i.Creature != null)
                        {
                            var creature = _context.Entry(i.Creature);
                            creature.Collection(c => c.ForcePowers).Load();
                            creature.Collection(c => c.Actions).Load();
                        }
                    }
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

        [HttpGet("shipencounter/{id:guid}")]
        public IActionResult GetShipEncounterById(Guid id)
        {
            try
            {
                var entity = _context.ShipEncounters
                     .Include(i => i.InitativeData)
                     .AsNoTracking()
                     .FirstOrDefault(x => x.Id == id);

                if (entity != null)
                {
                    var playerShips = _context.PlayerShips
                       .Include(c => c.Weapons).ThenInclude(w => w.Properties)
                       .Where(c => entity.InitativeData.Select(i => i.PlayerShipId).Contains(c.Id))
                       .AsNoTracking()
                       .ToList();
                    var enemyShips = _context.EnemyShips
                        .Include(c => c.Weapons).ThenInclude(w => w.Properties)
                        .Where(c => entity.InitativeData.Select(i => i.EnemyShipId).Contains(c.Id))
                        .AsNoTracking()
                        .ToList();
                    foreach (var i in entity.InitativeData)
                    {
                        if (i.PlayerShipId != null)
                            i.PlayerShip = playerShips.FirstOrDefault(c => c.Id == i.PlayerShipId);
                        if (i.EnemyShipId != null)
                            i.EnemyShip = enemyShips.FirstOrDefault(c => c.Id == i.EnemyShipId);
                    }
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

        [HttpGet("characters")]
        public IActionResult GetCharactersByIds([FromQuery] Guid[] ids)
        {
            try
            {
                var results = _context.GetQueryable<Character>()
                    .AsNoTracking();
                if (ids.Any())
                    results = results.Where(x => ids.Contains(x.Id));
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
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
            try
            {
                if (ids.Any())
                    return Ok(_context.GetQueryable<Creature>().AsNoTracking().Where(x => ids.Contains(x.Id)));
                return Ok(_context.Creatures);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        [HttpGet("forcepowers")]
        public IActionResult GetForcePowersByIds([FromQuery] Guid[] ids)
        {
            try
            {
                if (ids.Any())
                    return Ok(_context.GetQueryable<ForcePower>().AsNoTracking().Where(x => ids.Contains(x.Id)));
                return Ok(_context.ForcePowers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        [HttpGet("abilitiesids")]
        public IActionResult GetAbilitiesByIds([FromQuery] Guid[] ids)
        {
            try
            {
                if (ids.Any())
                    return Ok(_context.GetQueryable<Ability>().AsNoTracking().Where(x => ids.Contains(x.Id)));
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
