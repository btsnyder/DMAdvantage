﻿using AutoMapper;
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
            return GetAllEntities(paging, searching);
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

