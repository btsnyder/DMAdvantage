using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Shared.Models;
using DMAdvantage.Server;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class AbilityTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public AbilityTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/abilities");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllAbilities_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateAbility();

            var response = await client.GetAsync("/api/abilities");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilities = await response.ParseEntityList<AbilityResponse>();
            var abilitiesFromDb = await client.GetAllEntities<AbilityResponse>();
            abilities.Should().HaveCount(abilitiesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllAbilitiesWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var abilities = new List<AbilityResponse>();

            for (var i = 0; i < 25; i++)
            {
                var ability = Generation.AbilityRequest();
                ability.Name = $"{i:00000} - Ability";
                var abilityResponse = await client.CreateAbility(ability);
                abilities.Add(abilityResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/abilities?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilitiesResponse = await response.ParseEntityList<AbilityResponse>();

            abilitiesResponse.Should().HaveCount(paging.PageSize);
            abilitiesResponse[0].Id.Should().Be(abilities[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllAbilitiesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var abilities = new List<AbilityResponse>();

            for (var i = 0; i < 25; i++)
            {
                var ability = Generation.AbilityRequest();
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

            var response = await client.GetAsync($"/api/abilities?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var abilitiesResponse = await response.ParseEntityList<AbilityResponse>();

            abilitiesResponse.Should().BeEquivalentTo(abilities.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewAbility_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var ability = await client.CreateAbility();

            var addedAbility = await client.GetEntity<AbilityResponse>(ability.Id);
            Validation.CompareResponses(ability, addedAbility);
        }

        [Fact]
        public async Task Post_CreateNewInvalidAbility_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var ability = Generation.AbilityRequest();
            ability.Name = null;
            var response = await client.PostAsync("/api/abilities", ability);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_AbilityById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();

            var response = await client.GetAsync($"/api/abilities/{ability.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedAbility = await response.ParseEntity<AbilityResponse>();
            Validation.CompareResponses(ability, addedAbility);
        }

        [Fact]
        public async Task Put_AbilityById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();
            var abilityEdit = Generation.AbilityRequest();

            var response = await client.PutAsync($"api/abilities/{ability.Id}", abilityEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedAbility = await client.GetEntity<AbilityResponse>(ability.Id);
            addedAbility.Should().NotBeNull();
            Validation.CompareRequests(abilityEdit, addedAbility);
        }

        [Fact]
        public async Task Delete_AbilityById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var ability = await client.CreateAbility();

            var response = await client.DeleteAsync($"api/abilities/{ability.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var abilityLookup = await client.GetAsync($"api/abilities/{ability.Id}");
            abilityLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
