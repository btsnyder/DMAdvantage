using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Shared.Models;
using DMAdvantage.Server;
using FluentAssertions;
using System.Collections.Generic;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Query;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class TechPowerTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;
        public TechPowerTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/techpowers");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllTechPowers_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateTechPower();

            var response = await client.GetAsync("/api/techpowers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowers = await response.ParseEntityList<TechPower>();
            var techPowersFromDb = await client.GetAllEntities<TechPower>();
            techPowers.Should().HaveCount(techPowersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllTechPowersWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/techpowers?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowersResponse = await response.ParseEntityList<TechPower>();

            techPowersResponse.Should().HaveCount(paging.PageSize);
            techPowersResponse[0].Id.Should().Be(techPowers[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllTechPowersWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

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

            var response = await client.GetAsync($"/api/techpowers?{searching.GetQuery()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowers = await response.ParseEntityList<TechPower>();

            techPowers.Should().HaveCount(5);
            techPowers.TrueForAll(x => x.Name == "search").Should().Be(true);
            techPowers.TrueForAll(x => x.CastingPeriod == CastingPeriod.Hour).Should().Be(true);
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var techPower = await client.CreateTechPower();

            var addedTechPower = await client.GetEntity<TechPower>(techPower.Id);
            Validation.CompareEntities(techPower, addedTechPower);
        }

        [Fact]
        public async Task Post_CreateNewInvalidTechPower_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var techPower = Generation.TechPower();
            techPower.Name = null;
            var response = await client.PostAsync("/api/techpowers", techPower);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_TechPowerById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();

            var response = await client.GetAsync($"/api/techpowers/{techPower.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedTechPower = await response.ParseEntity<TechPower>();
            Validation.CompareEntities(techPower, addedTechPower);
        }

        [Fact]
        public async Task Put_TechPowerById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();
            var techPowerEdit = Generation.TechPower();
            techPowerEdit.Id = techPower.Id;

            var response = await client.PutAsync($"api/techpowers/{techPower.Id}", techPowerEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedTechPower = await client.GetEntity<TechPower>(techPower.Id);
            addedTechPower.Should().NotBeNull();
            Validation.CompareEntities(techPowerEdit, addedTechPower);
        }

        [Fact]
        public async Task Delete_TechPowerById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();

            var response = await client.DeleteAsync($"api/techpowers/{techPower.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var techPowerLookup = await client.GetAsync($"api/techpowers/{techPower.Id}");
            techPowerLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
