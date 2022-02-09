using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    public class ViewController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<ViewController> _logger;
        private readonly IMapper _mapper;

        public ViewController(IRepository repository,
            ILogger<ViewController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("encounter/{id:guid}")]
        public IActionResult GetEncounterById(Guid id)
        {
            try
            {
                var entity = _repository.GetEntityByIdWithoutUser<Encounter>(id);
                if (entity != null)
                    return Ok(_mapper.Map<EncounterResponse>(entity));
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
                var results = _repository.GetEntitiesByIdsWithoutUser<Character>(ids);
                return Ok(_mapper.Map<IEnumerable<CharacterResponse>>(results));
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
                var result = _repository.GetCharacterByPlayerNameWithoutUser(name);
                return Ok(_mapper.Map<CharacterResponse>(result));
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
                var results = _repository.GetEntitiesByIdsWithoutUser<Creature>(ids);
                return Ok(_mapper.Map<IEnumerable<CreatureResponse>>(results));
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
                var results = _repository.GetEntitiesByIdsWithoutUser<ForcePower>(ids);
                return Ok(_mapper.Map<IEnumerable<ForcePowerResponse>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }
    }
}
