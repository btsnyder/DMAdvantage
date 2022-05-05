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
    public class ClassesController : BaseEntityController<DMClass>
    {
        public ClassesController(DMContext context,
            ILogger<ClassesController> logger,
            UserManager<User> userManager) : 
            base(context, logger, userManager)
        {
        }

        [HttpGet]
        public IActionResult GetAllClasses([FromQuery] PagingParameters? paging = null, [FromQuery] NamedSearchParameters<DMClass>? searching = null)
        {
            if (searching == null) return GetAllEntities(paging);
            return GetAllEntities(searching, paging);
        }


        [HttpGet("{id:guid}")]
        public IActionResult GetClassById(Guid id)
        {
            return GetEntityById(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewClass([FromBody] DMClass request)
        {
            return await CreateNewEntity(request);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateClassById(Guid id, [FromBody] DMClass request)
        {
            return await UpdateEntityById(id, request);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteClassById(Guid id)
        {
            return DeleteEntityById(id);
        }
    }
}

