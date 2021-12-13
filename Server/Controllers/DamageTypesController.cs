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
    public class DamageTypesController : BaseEntityController<DamageType, DamageTypeResponse, DamageTypeRequest>
    {
        public DamageTypesController(IRepository repository,
            ILogger<DamageTypesController> logger,
            IMapper mapper,
            UserManager<User> userManager)
            : base(repository, logger, mapper, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllDamageTypes()
        {
            return GetAllEntities();
        }

        [HttpGet("{id}")]
        public IActionResult GetDamageTypeById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewDamageType([FromBody] DamageTypeRequest request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDamageTypeById(Guid id, [FromBody] DamageTypeRequest request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDamageTypeById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

