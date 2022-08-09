using System.Collections.Generic;
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

namespace DMAdvantage.UnitTests.Controllers
{
    public class ForcePowerTests
    {
        private readonly TestServer _server;
        public ForcePowerTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllForcePowers_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateForcePower();

            var response = await client.GetAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowers = await response.ParseEntityList<ForcePower>();
            var forcePowersFromDb = await client.GetAllEntities<ForcePower>();
            forcePowers.Should().HaveCount(forcePowersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllForcePowersWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var forcePowers = new List<ForcePower>();

            for (var i = 0; i < 25; i++)
            {
                var forcePower = Generation.ForcePower();
                forcePower.Level = 0;
                forcePower.Name = $"{i:00000} - ForcePower";
                var forcePowerResponse = await client.CreateForcePower(forcePower);
                forcePowers.Add(forcePowerResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowersResponse = await response.ParseEntityList<ForcePower>();

            forcePowersResponse.Should().HaveCount(paging.PageSize);
            forcePowersResponse[0].Id.Should().Be(forcePowers[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllForcePowersWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            for (var i = 0; i < 25; i++)
            {
                var forcePower = Generation.ForcePower();
                switch (i)
                {
                    case < 5:
                        forcePower.Alignment = ForceAlignment.Dark;
                        forcePower.Name = "search";
                        break;
                    case < 15:
                        forcePower.Name = "not found";
                        break;
                    default:
                        forcePower.Name = "search";
                        forcePower.Alignment = ForceAlignment.Light;
                        break;
                }
                await client.CreateForcePower(forcePower);
            }

            var searching = new ForcePowerSearchParameters
            {
                Search = "search",
                Alignments = new[] { ForceAlignment.Dark }
            };

            var response = await client.GetAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}?{searching.GetQuery()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var forcePowersResponse = await response.ParseEntityList<ForcePower>();

            forcePowersResponse.Should().HaveCount(5);
            forcePowersResponse.TrueForAll(x => x.Name == "search").Should().Be(true);
            forcePowersResponse.TrueForAll(x => x.Alignment == ForceAlignment.Dark).Should().Be(true);
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var forcePower = await client.CreateForcePower();

            var addedForcePower = await client.GetEntity<ForcePower>(forcePower.Id);
            Validation.CompareEntities(forcePower, addedForcePower);
        }

        [Fact]
        public async Task Post_CreateNewInvalidForcePower_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var forcePower = Generation.ForcePower();
            forcePower.Name = null;
            var response = await client.PostAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}", forcePower);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_ForcePowerById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();

            var response = await client.GetAsync($"/api/{GenericHelpers.GetPath<ForcePower>()}/{forcePower.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedForcePower = await response.ParseEntity<ForcePower>();
            Validation.CompareEntities(forcePower, addedForcePower);
        }

        [Fact]
        public async Task Put_ForcePowerById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();
            var forcePowerEdit = Generation.ForcePower();
            forcePowerEdit.Id = forcePower.Id;

            var response = await client.PutAsync($"api/{GenericHelpers.GetPath<ForcePower>()}/{forcePower.Id}", forcePowerEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedForcePower = await client.GetEntity<ForcePower>(forcePower.Id);
            addedForcePower.Should().NotBeNull();
            Validation.CompareEntities(forcePowerEdit, addedForcePower);
        }

        [Fact]
        public async Task Delete_ForcePowerById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var forcePower = await client.CreateForcePower();

            var response = await client.DeleteAsync($"api/{GenericHelpers.GetPath<ForcePower>()}/{forcePower.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var forcePowerLookup = await client.GetAsync($"api/{GenericHelpers.GetPath<ForcePower>()}/{forcePower.Id}");
            forcePowerLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
