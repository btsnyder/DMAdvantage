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
    public class WeaponPropertiesController : BaseEntityController<WeaponProperty>
    {
        public WeaponPropertiesController(DMContext context,
            ILogger<WeaponPropertiesController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
            apiPath = "weaponproperties";
        }

        [HttpGet]
        public IActionResult GetAllWeaponProperties([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<WeaponProperty>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetWeaponPropertyById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewWeaponProperty([FromBody] WeaponProperty request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateWeaponPropertyById(Guid id, [FromBody] WeaponProperty request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteWeaponPropertyById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

