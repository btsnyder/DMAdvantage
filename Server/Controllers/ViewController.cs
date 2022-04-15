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
        public IActionResult GetEncounterById(Guid id)
        {
            try
            {
                var entity = _context.Encounters
                     .Include(i => i.InitativeData)
                     .AsNoTracking()
                     .FirstOrDefault(x => x.Id == id);
               
 
                if (entity != null)
                {
                    var characters = _context.Characters
                       .Include(c => c.Abilities)
                       .Include(c => c.Class)
                       .Include(c => c.ForcePowers)
                       .Include(c => c.Weapons).ThenInclude(w => w.Properties)
                       .Where(c => entity.InitativeData.Select(i => i.CharacterId).Contains(c.Id))
                       .AsNoTracking()
                       .ToList();
                    var creatures = _context.Creatures
                        .Include(c => c.ForcePowers)
                        .Include(c => c.Actions)
                        .Where(c => entity.InitativeData.Select(i => i.CreatureId).Contains(c.Id))
                        .AsNoTracking()
                        .ToList();
                    foreach (var i in entity.InitativeData)
                    {
                        if (i.CharacterId != null)
                            i.Character = characters.FirstOrDefault(c => c.Id == i.CharacterId);
                        if (i.CreatureId != null)
                            i.Creature = creatures.FirstOrDefault(c => c.Id == i.CreatureId);
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
