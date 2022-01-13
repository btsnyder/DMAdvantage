using TestEngineering.Mocks;
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
    public class TechPowerTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;
        public TechPowerTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
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
            var techPowers = await response.ParseEntityList<TechPowerResponse>();
            var techPowersFromDb = await client.GetAllEntities<TechPowerResponse>();
            techPowers.Should().HaveCount(techPowersFromDb.Count);
        }

        public async Task Get_AllTechPowersWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var techPowers = new List<TechPowerResponse>();

            for (int i = 0; i < 25; i++)
            {
                var techPower = Generation.TechPowerRequest();
                techPower.Level = 0;
                techPower.Name = $"{string.Format("{0:00000}", i)} - TechPower";
                var techPowerResponse = await client.CreateTechPower(techPower);
                if (techPowerResponse != null)
                    techPowers.Add(techPowerResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/techpowers?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var techPowersResponse = await response.ParseEntityList<TechPowerResponse>();

            techPowersResponse.Should().HaveCount(paging.PageSize);
            techPowersResponse[0].Id.Should().Be(techPowers[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var techPower = await client.CreateTechPower();

            var addedTechPower = await client.GetEntity<TechPowerResponse>(techPower.Id);
            Validation.CompareData(techPower, addedTechPower);
        }

        [Fact]
        public async Task Post_CreateNewInvalidTechPower_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var techPower = Generation.TechPowerRequest();
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
            var addedTechPower = await response.ParseEntity<TechPowerResponse>();
            Validation.CompareData(techPower, addedTechPower);
        }

        [Fact]
        public async Task Put_TechPowerById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var techPower = await client.CreateTechPower();
            var techPowerEdit = Generation.TechPowerRequest();

            var response = await client.PutAsync($"api/techpowers/{techPower.Id}", techPowerEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedTechPower = await client.GetEntity<TechPowerResponse>(techPower.Id);
            addedTechPower.Should().NotBeNull();
            Validation.CompareData(techPowerEdit, addedTechPower);
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
