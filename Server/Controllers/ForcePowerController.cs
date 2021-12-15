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
    public class ForcePowersController : BaseEntityController<ForcePower, ForcePowerResponse, ForcePowerRequest>
    {
        public ForcePowersController(IRepository repository,
            ILogger<ForcePowersController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllForcePowers([FromQuery] PagingParameters? paging = null)
        {
            if (paging == null)
                return GetAllEntities();
            else
                return GetAllEntities(paging);
        }

        [HttpGet("{id}")]
        public IActionResult GetForcePowerById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewForcePower([FromBody] ForcePowerRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForcePowerById(Guid id, [FromBody] ForcePowerRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteForcePowerById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

