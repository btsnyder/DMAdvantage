using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EncountersController : BaseEntityController<Encounter, EncounterResponse, EncounterRequest>
    {
        public EncountersController(IRepository repository,
            ILogger<EncountersController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllEncounters()
        {
            return GetAllEntities();
        }

        [HttpGet("{id}")]
        public IActionResult GetEncounterById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEncounter([FromBody] EncounterRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEncounterById(Guid id, [FromBody] EncounterRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEncounterById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

