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
    public class CreaturesController : BaseEntityController<Creature, CreatureResponse, CreatureRequest>
    {
        public CreaturesController(IRepository repository,
            ILogger<CreaturesController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCreatures([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Creature>? searching = null)
        {
            return GetAllEntities(paging, searching);
        }

        [HttpGet("{id}")]
        public IActionResult GetCreatureById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCreature([FromBody] CreatureRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCreatureById(Guid id, [FromBody] CreatureRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCreatureById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

