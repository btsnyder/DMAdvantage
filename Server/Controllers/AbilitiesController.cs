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
    public class AbilitiesController : BaseEntityController<Ability>
    {
        public AbilitiesController(DMContext context,
            ILogger<AbilitiesController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllAbilities([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Ability>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetAbilityById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewAbility([FromBody] Ability request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAbilityById(Guid id, [FromBody] Ability request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteAbilityById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

