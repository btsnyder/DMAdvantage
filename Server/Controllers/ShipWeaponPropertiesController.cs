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
    public class ShipWeaponPropertiesController : BaseEntityController<ShipWeaponProperty>
    {
        public ShipWeaponPropertiesController(DMContext context,
            ILogger<ShipWeaponPropertiesController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllShipWeaponProperties([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<ShipWeaponProperty>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetShipWeaponPropertyById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipNewWeaponProperty([FromBody] ShipWeaponProperty request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateShipWeaponPropertyById(Guid id, [FromBody] ShipWeaponProperty request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteShipWeaponPropertyById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

