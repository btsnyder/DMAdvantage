using System.Text.Json;
using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CharactersController : BaseEntityController<Character, CharacterResponse, CharacterRequest>
    {
        public CharactersController(IRepository repository,
            ILogger<CharactersController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCharacters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Character>? searching = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                var search = searching?.Search ?? string.Empty;
                var query = _repository.Context.Characters
                    .Include(c => c.Abilities)
                    .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search))
                    .AsNoTracking();
                
                var characters = query.ToList().OrderBy(c => c.OrderBy());

                if (paging == null)
                    return Ok(_mapper.Map<IEnumerable<CharacterResponse>>(characters));
                
                var pagedResults = PagedList<Character>.ToPagedList(characters.ToList(), paging);

                var metadata = new PagedData
                {
                    TotalCount = pagedResults.TotalCount,
                    PageSize = pagedResults.PageSize,
                    CurrentPage = pagedResults.CurrentPage,
                    TotalPages = pagedResults.TotalPages,
                    HasNext = pagedResults.HasNext,
                    HasPrevious = pagedResults.HasPrevious
                };

                Response.Headers.Add(PagedData.Header, JsonSerializer.Serialize(metadata));

                return Ok(_mapper.Map<IEnumerable<CharacterResponse>>(pagedResults));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }


        [HttpGet("{id:guid}")]
        public IActionResult GetCharacterById(Guid id)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                var character = _repository.Context.Characters
                    .Include(c => c.Abilities).AsNoTracking()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (character == null) return NotFound();
                return Ok(_mapper.Map<CharacterResponse>(character));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entity: {ex}");
                return BadRequest("Failed to return entity");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCharacter([FromBody] CharacterRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var character = await CreateNewCharacterInContext(request);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/characters/{character.Id}", _mapper.Map<CharacterResponse>(character));
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

        private async Task<Character> CreateNewCharacterInContext(CharacterRequest request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            var entry = _repository.Context.Add(new Character());
            entry.Entity.User = currentUser;
            entry.CurrentValues.SetValues(request);
            var abilities =
                _repository.Context.Abilities
                    .Where(x => request.Abilities.Select(a => a.Id).Contains(x.Id))
                    .ToList();
            entry.Entity.Abilities = abilities;
            entry.Entity.WeaponsCache = JsonSerializer.Serialize(request.Weapons);
            return entry.Entity;
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCharacterById(Guid id, [FromBody] CharacterRequest request)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;


                var entityFromRepo = _repository.Context.Characters
                    .Include(c => c.Abilities)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    var character = await CreateNewCharacterInContext(request);

                    return Created($"/api/characters/{character.Id}", _mapper.Map<CharacterResponse>(character));
                }

                var entry = _repository.Context.Entry(entityFromRepo);
                entry.CurrentValues.SetValues(request);
                var abilities =
                    _repository.Context.Abilities
                        .Where(x => request.Abilities.Select(a => a.Id).Contains(x.Id))
                        .ToList();
                entry.Entity.Abilities = abilities;
                entry.Entity.WeaponsCache = JsonSerializer.Serialize(request.Weapons);
                _repository.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCharacterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

