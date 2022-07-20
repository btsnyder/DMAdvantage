using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TestEngineering;
using Xunit;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class TechPowerTests
    {
        private readonly TestServer _server;
        public TechPowerTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllTechPowers_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateTechPower();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowers = await response.ParseEntityList<TechPower>();
            var techPowersFromDb = await client.GetAllEntities<TechPower>();
            techPowers.Should().HaveCount(techPowersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllTechPowersWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var techPowers = new List<TechPower>();

            for (var i = 0; i < 25; i++)
            {
                var techPower = Generation.TechPower();
                techPower.Level = 0;
                techPower.Name = $"{i:00000} - TechPower";
                var techPowerResponse = await client.CreateTechPower(techPower);
                techPowers.Add(techPowerResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowersResponse = await response.ParseEntityList<TechPower>();

            techPowersResponse.Should().HaveCount(paging.PageSize);
            techPowersResponse[0].Id.Should().Be(techPowers[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllTechPowersWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            for (var i = 0; i < 25; i++)
            {
                var techPower = Generation.TechPower();
                switch (i)
                {
                    case < 5:
                        techPower.CastingPeriod = CastingPeriod.Hour;
                        techPower.Name = "search";
                        break;
                    case < 15:
                        techPower.Name = "not found";
                        break;
                    default:
                        techPower.Name = "search";
                        techPower.CastingPeriod = CastingPeriod.EightHours;
                        break;
                }
                await client.CreateTechPower(techPower);
            }

            var searching = new TechPowerSearchParameters
            {
                Search = "search",
                CastingPeriods = new[] { CastingPeriod.Hour }
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}?{searching.GetQuery()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowers = await response.ParseEntityList<TechPower>();

            techPowers.Should().HaveCount(5);
            techPowers.TrueForAll(x => x.Name == "search").Should().Be(true);
            techPowers.TrueForAll(x => x.CastingPeriod == CastingPeriod.Hour).Should().Be(true);
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var techPower = await client.CreateTechPower();

            var addedTechPower = await client.GetEntity<TechPower>(techPower.Id);
            Validation.CompareEntities(techPower, addedTechPower);
        }

        [Fact]
        public async Task Post_CreateNewInvalidTechPower_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var techPower = Generation.TechPower();
            techPower.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}", techPower);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_TechPowerById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<TechPower>()}/{techPower.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedTechPower = await response.ParseEntity<TechPower>();
            Validation.CompareEntities(techPower, addedTechPower);
        }

        [Fact]
        public async Task Put_TechPowerById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();
            var techPowerEdit = Generation.TechPower();
            techPowerEdit.Id = techPower.Id;

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<TechPower>()}/{techPower.Id}", techPowerEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedTechPower = await client.GetEntity<TechPower>(techPower.Id);
            addedTechPower.Should().NotBeNull();
            Validation.CompareEntities(techPowerEdit, addedTechPower);
        }

        [Fact]
        public async Task Delete_TechPowerById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<TechPower>()}/{techPower.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var techPowerLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<TechPower>()}/{techPower.Id}");
            techPowerLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
