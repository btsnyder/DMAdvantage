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
    public class EncountersController : BaseEntityController<Encounter>
    {
        public EncountersController(DMContext context,
            ILogger<EncountersController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllEncounters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Encounter>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetEncounterById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEncounter([FromBody] Encounter request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEncounterById(Guid id, [FromBody] Encounter request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEncounterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

