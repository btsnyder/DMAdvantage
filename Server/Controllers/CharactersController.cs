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
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CharactersController : BaseEntityController<Character, CharacterResponse, CharacterRequest>
    {
        public CharactersController(IRepository repository,
            ILogger<CharactersController> logger,
            IMapper mapper,
            UserManager<User> userManager) : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCharacters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Character>? searching = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching?.Search ?? string.Empty;

                var characters = _repository.Context.Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search))
                    .AsNoTracking().ToList()
                    .OrderBy(c => c.OrderBy());
                
                if (paging == null)
                    return Ok(_mapper.Map<IEnumerable<CharacterResponse>>(characters));

                var pagedResults = PagedList<Character>.ToPagedList(characters, paging);
                Response.SetPagedHeader(pagedResults);
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
                    .Include(c => c.Abilities)
                    .Include(c => c.Class).AsNoTracking()
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCharacterById(Guid id, [FromBody] CharacterRequest request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _repository.Context.Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    var character = await CreateNewCharacterInContext(request);

                    return Created($"/api/characters/{character.Id}", _mapper.Map<CharacterResponse>(character));
                }

                await CreateNewCharacterInContext(request, entityFromRepo);
                _repository.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Character> CreateNewCharacterInContext(CharacterRequest request, Character? character = null)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            EntityEntry<Character> entry;
            if (character == null)
                entry = _repository.Context.Add(new Character());
            else
                entry = _repository.Context.Entry(character);
            entry.Entity.User = currentUser;
            entry.CurrentValues.SetValues(request);
            var abilities = _repository.Context.Abilities
                .Where(x => request.Abilities.Select(a => a.Id).Contains(x.Id)).ToList();
            entry.Entity.Abilities = abilities;
            var dmclass = _repository.Context.DMClasses
                .FirstOrDefault(x => request.Class.Id == x.Id);
            entry.Entity.Class = dmclass;
            entry.Entity.WeaponsCache = JsonSerializer.Serialize(request.Weapons);
            return entry.Entity;
        }


        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCharacterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

