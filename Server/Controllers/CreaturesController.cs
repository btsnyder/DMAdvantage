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
using System.Text.Json;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CreaturesController : BaseEntityController<Creature>
    {
        public CreaturesController(DMContext context,
            ILogger<CreaturesController> logger,
            UserManager<User> userManager)
            : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCreatures([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Creature>? searching = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching?.Search ?? string.Empty;

                var creatures = _context.Creatures
                    .Include(c => c.ForcePowers)
                    .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search))
                    .AsNoTracking().ToList()
                    .OrderBy(c => c.OrderBy());

                if (paging == null)
                    return Ok(creatures);

                var pagedResults = PagedList<Creature>.ToPagedList(creatures, paging);
                Response.SetPagedHeader(pagedResults);
                return Ok(pagedResults);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetCreatureById(Guid id)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                var creature = _context.Creatures
                    .Include(c => c.ForcePowers)
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (creature == null) return NotFound();
                return Ok(creature);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entity: {ex}");
                return BadRequest("Failed to return entity");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCreature([FromBody] Creature request)
        {
            return await CreateNewEntity(request);
        }
       

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCreatureById(Guid id, [FromBody] Creature request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.Creatures
                    .Include(c => c.ForcePowers)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateCreature(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Creature> UpdateCreature(Creature creatureFromRepo, Creature request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entity = _context.Entry(creatureFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;
            var forcePowers = _context.ForcePowers
                .Where(x => request.ForcePowers.Select(f => f.Id).Contains(x.Id)).ToList();
            entity.Entity.ForcePowers = forcePowers;
            return entity.Entity;
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCreatureById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

