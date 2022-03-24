﻿using System.Text.Json;
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
    public class CharactersController : BaseEntityController<Character>
    {
        public CharactersController(DMContext context,
            ILogger<CharactersController> logger,
            UserManager<User> userManager) : base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllCharacters([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<Character>? searching = null)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching?.Search ?? string.Empty;

                var characters = _context.Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .Include(c => c.ForcePowers)
                    .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search))
                    .AsNoTracking().ToList()
                    .OrderBy(c => c.OrderBy());
                
                if (paging == null)
                    return Ok(characters);

                var pagedResults = PagedList<Character>.ToPagedList(characters, paging);
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
        public IActionResult GetCharacterById(Guid id)
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;

                var character = _context.Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .Include(c => c.ForcePowers)
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (character == null) return NotFound();
                return Ok(character);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entity: {ex}");
                return BadRequest("Failed to return entity");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCharacter([FromBody] Character request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCharacterById(Guid id, [FromBody] Character request)
        {
            try
            {
                var username = User.Identity?.Name;
                if (username == null)
                    throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

                var entityFromRepo = _context.Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .Include(c => c.ForcePowers)
                    .FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);

                if (entityFromRepo == null)
                {
                    request.Id = id;
                    return await CreateNewEntity(request);
                }

                await UpdateCharacter(entityFromRepo, request);
                _context.SaveAll();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update entity: {ex}");
            }

            return BadRequest("Failed to update entity.");
        }

        private async Task<Character> UpdateCharacter(Character characterFromRepo, Character request)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (currentUser == null)
                throw new UnauthorizedAccessException($"Could not find user: {User.Identity?.Name}");

            var entity = _context.Entry(characterFromRepo);
            request.Id = entity.Entity.Id;
            entity.CurrentValues.SetValues(request);
            entity.Entity.User = currentUser;
            var abilities = _context.Abilities
                .Where(x => request.Abilities.Select(a => a.Id).Contains(x.Id)).ToList();
            entity.Entity.Abilities = abilities;
            var forcePowers = _context.ForcePowers
                .Where(x => request.ForcePowers.Select(f => f.Id).Contains(x.Id)).ToList();
            entity.Entity.ForcePowers = forcePowers;
            var dmclass = _context.DMClasses
                .FirstOrDefault(x => request.Class != null && request.Class.Id == x.Id);
            entity.Entity.Class = dmclass;
            return entity.Entity;
        }


        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCharacterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

