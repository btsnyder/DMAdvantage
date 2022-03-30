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
    public class WeaponsController : BaseEntityController<Weapon>
    {
        public WeaponsController(DMContext context,
            ILogger<WeaponsController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllWeapons([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Weapon>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetWeaponById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewWeapon([FromBody] Weapon request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateWeaponById(Guid id, [FromBody] Weapon request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.Weapons
                    .Include(w => w.Properties)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateWeapon(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Weapon> UpdateWeapon(Weapon fromContext, Weapon request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entity = _context.Entry(fromContext);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;
            var properties = _context.WeaponProperties
                .Where(x => request.Properties.Select(f => f.Id).Contains(x.Id)).ToList();
            entity.Entity.Properties = properties;
            return entity.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteWeaponById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

