using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            return GetAllEntities(paging, searching);
        }


        [HttpGet("{id}")]
        public IActionResult GetCharacterById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCharacter([FromBody] CharacterRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCharacterById(Guid id, [FromBody] CharacterRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCharacterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

