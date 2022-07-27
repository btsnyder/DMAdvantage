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
    [Route($"api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlayerShipsController : BaseEntityController<PlayerShip>
    {
        public PlayerShipsController(DMContext context,
            ILogger<PlayerShipsController> logger,
            UserManager<User> userManager) : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllShips([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<PlayerShip>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetShipById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewShip([FromBody] PlayerShip request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateShipById(Guid id, [FromBody] PlayerShip request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.GetQueryable<PlayerShip>().GetEntityByIdAndUser(username, id);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateShip(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<PlayerShip> UpdateShip(PlayerShip characterFromRepo, PlayerShip request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entity = _context.Entry(characterFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;
            entity.Entity.Weapons = _context.GetEntitiesByIds(request.Weapons);
            return entity.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteShipById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

