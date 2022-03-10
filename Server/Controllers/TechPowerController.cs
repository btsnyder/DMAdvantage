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

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TechPowersController : BaseEntityController<TechPower, TechPowerResponse, TechPowerRequest>
    {
        public TechPowersController(IRepository repository,
            ILogger<TechPowersController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllTechPowers([FromQuery] PagingParameters? paging = null, [FromQuery] TechPowerSearchParameters? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var search = searching.Search;

                IQueryable<TechPower> query;
                if (search == null)
                    query = _repository.Context.TechPowers
                        .Where(c => c.User != null && c.User.UserName == username)
                        .AsNoTracking();
                else
                    query = _repository.Context.TechPowers
                        .Where(c => c.User != null && c.User.UserName == username && c.Name != null && c.Name.ToLower().Contains(search.ToLower()))
                        .AsNoTracking();

                if (searching.Levels.Any())
                    query = query.Where(c => searching.Levels.Contains(c.Level));
                if (searching.CastingPeriods.Any())
                    query = query.Where(c => searching.CastingPeriods.Contains(c.CastingPeriod));
                if (searching.Ranges.Any())
                    query = query.Where(c => searching.Ranges.Contains(c.Range));

                var entities = query.ToList().OrderBy(c => c.OrderBy());

                if (paging == null)
                    return Ok(_mapper.Map<IEnumerable<TechPowerResponse>>(entities));

                var pagedResults = PagedList<TechPower>.ToPagedList(entities, paging);
                Response.SetPagedHeader(pagedResults);
                return Ok(_mapper.Map<IEnumerable<TechPowerResponse>>(pagedResults));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return entities: {ex}");
                return BadRequest("Failed to return entities");
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetTechPowerById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewTechPower([FromBody] TechPowerRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTechPowerById(Guid id, [FromBody] TechPowerRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteTechPowerById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

