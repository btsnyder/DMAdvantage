﻿using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Shared.Models;
using DMAdvantage.Server;
using FluentAssertions;
using System.Collections.Generic;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class DamageTypeTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;
        public DamageTypeTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/damagetypes");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllDamageTypes_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateDamageType();

            var response = await client.GetAsync("/api/damagetypes");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var damageTypes = await response.ParseEntityList<DamageTypeResponse>();
            var damageTypesFromDb = await client.GetAllEntities<DamageTypeResponse>();
            damageTypes.Should().HaveCount(damageTypesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllDamageTypesWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var damageTypes = new List<DamageTypeResponse>();

            for (int i = 0; i < 25; i++)
            {
                var damageType = Generation.DamageTypeRequest();
                damageType.Name = $"{string.Format("{0:00000}", i)} - DamageType";
                var damageTypeResponse = await client.CreateDamageType(damageType);
                if (damageTypeResponse != null)
                    damageTypes.Add(damageTypeResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/damagetypes?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var damageTypesResponse = await response.ParseEntityList<DamageTypeResponse>();

            damageTypesResponse.Should().HaveCount(paging.PageSize);
            damageTypesResponse[0].Id.Should().Be(damageTypes[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Post_CreateNewDamageType_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var damageType = await client.CreateDamageType();

            var addedDamageType = await client.GetEntity<DamageTypeResponse>(damageType.Id);
            Validation.CompareData(damageType, addedDamageType);
        }

        [Fact]
        public async Task Post_CreateNewInvalidDamageType_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var damageType = Generation.DamageTypeRequest();
            damageType.Name = null;
            var response = await client.PostAsync("/api/damagetypes", damageType);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_DamageTypeById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var damageType = await client.CreateDamageType();

            var response = await client.GetAsync($"/api/damagetypes/{damageType.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedDamageType = await response.ParseEntity<DamageTypeResponse>();
            Validation.CompareData(damageType, addedDamageType);
        }

        [Fact]
        public async Task Put_DamageTypeById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var damageType = await client.CreateDamageType();
            var damageTypeEdit = Generation.DamageTypeRequest();

            var response = await client.PutAsync($"api/damagetypes/{damageType.Id}", damageTypeEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedDamageType = await client.GetEntity<DamageTypeResponse>(damageType.Id);
            addedDamageType.Should().NotBeNull();
            Validation.CompareData(damageTypeEdit, addedDamageType);
        }

        [Fact]
        public async Task Delete_DamageTypeById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var damageType = await client.CreateDamageType();

            var response = await client.DeleteAsync($"api/damagetypes/{damageType.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var damageTypeLookup = await client.GetAsync($"api/damagetypes/{damageType.Id}");
            damageTypeLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
