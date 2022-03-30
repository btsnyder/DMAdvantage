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
                    .Include(e => e.InitativeData).ThenInclude(i => i.Character).ThenInclude(c => c.Abilities)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Character).ThenInclude(c => c.Class)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Character).ThenInclude(c => c.ForcePowers)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Character).ThenInclude(c => c.Weapons).ThenInclude(w => w.Properties)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Creature).ThenInclude(c => c.ForcePowers)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Creature).ThenInclude(c => c.Actions)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
                if (entity != null)
                    return Ok(entity);
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
