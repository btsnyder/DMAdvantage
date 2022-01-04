using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    public class EncountersViewController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<EncountersController> _logger;
        private readonly IMapper _mapper;

        public EncountersViewController(IRepository repository,
            ILogger<EncountersController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("encounter/{id}")]
        public IActionResult GetEncounterById(Guid id)
        {
            try
            {
                var entity = _repository.GetEntityByIdWithoutUser<Encounter>(id);
                if (entity != null)
                    return Ok(_mapper.Map<EncounterResponse>(entity));
                else return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return encounter: {ex}");
                return BadRequest($"Failed to return encounter");
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
                return BadRequest($"Failed to return entities");
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
                return BadRequest($"Failed to return entities");
            }
        }
    }
}
