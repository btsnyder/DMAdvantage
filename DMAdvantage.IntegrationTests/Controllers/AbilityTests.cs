using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TestEngineering;
using Xunit;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class AbilityTests
    {
        private readonly TestServer _server;

        public AbilityTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }   

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllAbilities_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateAbility();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilities = await response.ParseEntityList<Ability>();
            var abilitiesFromDb = await client.GetAllEntities<Ability>();
            abilities.Should().HaveCount(abilitiesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllAbilitiesWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var abilities = new List<Ability>();

            for (var i = 0; i < 25; i++)
            {
                var ability = Generation.Ability();
                ability.Name = $"{i:00000} - Ability";
                var abilityResponse = await client.CreateAbility(ability);
                abilities.Add(abilityResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilitiesResponse = await response.ParseEntityList<Ability>();

            abilitiesResponse.Should().HaveCount(paging.PageSize);
            abilitiesResponse[0].Id.Should().Be(abilities[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllAbilitiesWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var abilities = new List<Ability>();

            for (var i = 0; i < 25; i++)
            {
                var ability = Generation.Ability();
                ability.Name = $"{i:00000} - Ability";
                if (Faker.Boolean.Random())
                    ability.Name += "Found";
                var abilityResponse = await client.CreateAbility(ability);
                abilities.Add(abilityResponse);
            }

            var search = new NamedSearchParameters<Ability>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilitiesResponse = await response.ParseEntityList<Ability>();

            foreach (var ability in abilitiesResponse)
            {
                ability.User = abilities.First().User;
            }

            abilitiesResponse.Should().BeEquivalentTo(abilities.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewAbility_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var ability = await client.CreateAbility();

            var addedAbility = await client.GetEntity<Ability>(ability.Id);
            Validation.CompareEntities(ability, addedAbility);
        }

        [Fact]
        public async Task Post_CreateNewInvalidAbility_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var ability = Generation.Ability();
            ability.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}", ability);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_AbilityById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Ability>()}/{ability.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedAbility = await response.ParseEntity<Ability>();
            Validation.CompareEntities(ability, addedAbility);
        }

        [Fact]
        public async Task Put_AbilityById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();
            var abilityEdit = Generation.Ability();

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<Ability>()}/{ability.Id}", abilityEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedAbility = await client.GetEntity<Ability>(ability.Id);
            addedAbility.Should().NotBeNull();
            abilityEdit.Id = ability.Id;
            Validation.CompareEntities(abilityEdit, addedAbility);
        }

        [Fact]
        public async Task Delete_AbilityById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<Ability>()}/{ability.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var abilityLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<Ability>()}/{ability.Id}");
            abilityLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
