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

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class ForcePowerTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;
        public ForcePowerTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/forcepowers");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllForcePowers_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateForcePower();

            var response = await client.GetAsync("/api/forcepowers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowers = await response.ParseEntityList<ForcePowerResponse>();
            var forcePowersFromDb = await client.GetAllEntities<ForcePowerResponse>();
            forcePowers.Should().HaveCount(forcePowersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllForcePowersWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var forcePowers = new List<ForcePowerResponse>();

            for (int i = 0; i < 25; i++)
            {
                var forcePower = Generation.ForcePowerRequest();
                forcePower.Level = 0;
                forcePower.Name = $"{string.Format("{0:00000}", i)} - ForcePower";
                var forcePowerResponse = await client.CreateForcePower(forcePower);
                if (forcePowerResponse != null)
                    forcePowers.Add(forcePowerResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/forcepowers?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowersResponse = await response.ParseEntityList<ForcePowerResponse>();

            forcePowersResponse.Should().HaveCount(paging.PageSize);
            forcePowersResponse[0].Id.Should().Be(forcePowers[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllForcePowersWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var forcePowers = new List<ForcePowerResponse>();

            for (int i = 0; i < 25; i++)
            {
                var forcePower = Generation.ForcePowerRequest();
                if (i < 5)
                {
                    forcePower.Alignment = ForceAlignment.Dark;
                    forcePower.Name = "search";
                }
                else if (i < 15)
                {
                    forcePower.Name = "not found";
                }
                else
                {
                    forcePower.Name = "search";
                    forcePower.Alignment = ForceAlignment.Light;
                }
                var forcePowerResponse = await client.CreateForcePower(forcePower);
                if (forcePowerResponse != null)
                    forcePowers.Add(forcePowerResponse);
            }

            var searching = new ForcePowerSearchParameters
            {
                Search = "search",
                Alignments = new ForceAlignment[] { ForceAlignment.Dark }
            };

            var response = await client.GetAsync($"/api/forcepowers?{searching.GetQuery()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowersResponse = await response.ParseEntityList<ForcePowerResponse>();

            forcePowersResponse.Should().HaveCount(5);
            forcePowersResponse.TrueForAll(x => x.Name == "search");
            forcePowersResponse.TrueForAll(x => x.Alignment == ForceAlignment.Dark);
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var forcePower = await client.CreateForcePower();

            var addedForcePower = await client.GetEntity<ForcePowerResponse>(forcePower.Id);
            Validation.CompareData(forcePower, addedForcePower);
        }

        [Fact]
        public async Task Post_CreateNewInvalidForcePower_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var forcePower = Generation.ForcePowerRequest();
            forcePower.Name = null;
            var response = await client.PostAsync("/api/forcepowers", forcePower);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_ForcePowerById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();

            var response = await client.GetAsync($"/api/forcepowers/{forcePower.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedForcePower = await response.ParseEntity<ForcePowerResponse>();
            Validation.CompareData(forcePower, addedForcePower);
        }

        [Fact]
        public async Task Put_ForcePowerById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();
            var forcePowerEdit = Generation.ForcePowerRequest();

            var response = await client.PutAsync($"api/forcepowers/{forcePower.Id}", forcePowerEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedForcePower = await client.GetEntity<ForcePowerResponse>(forcePower.Id);
            addedForcePower.Should().NotBeNull();
            Validation.CompareData(forcePowerEdit, addedForcePower);
        }

        [Fact]
        public async Task Delete_ForcePowerById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();

            var response = await client.DeleteAsync($"api/forcepowers/{forcePower.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var forcePowerLookup = await client.GetAsync($"api/forcepowers/{forcePower.Id}");
            forcePowerLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
