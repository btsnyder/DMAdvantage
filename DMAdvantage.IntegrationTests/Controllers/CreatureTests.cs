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
    public class CreatureTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public CreatureTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/creatures");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllCreatures_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateCreature();

            var response = await client.GetAsync("/api/creatures");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var creatures = await response.ParseEntityList<Creature>();
            var creaturesFromDb = await client.GetAllEntities<Creature>();
            creatures.Should().HaveCount(creaturesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllCreaturesWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creatures = new List<Creature>();

            for (var i = 0; i < 25; i++)
            {
                var creature = Generation.Creature();
                creature.Name = $"{i:00000} - Creature";
                var creatureResponse = await client.CreateCreature(creature);
                creatures.Add(creatureResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/creatures?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var creaturesResponse = await response.ParseEntityList<Creature>();

            creaturesResponse.Should().HaveCount(paging.PageSize);
            creaturesResponse[0].Id.Should().Be(creatures[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllCreaturesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creatures = new List<Creature>();

            for (var i = 0; i < 25; i++)
            {
                var creature = Generation.Creature();
                creature.Name = $"{i:00000} - Creature";
                if (Faker.Boolean.Random())
                    creature.Name += "Found";
                var creatureResponse = await client.CreateCreature(creature);
                creatures.Add(creatureResponse);
            }

            var search = new NamedSearchParameters<Creature>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/creatures?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var creaturesResponse = await response.ParseEntityList<Creature>();

            foreach (var creature in creaturesResponse)
            {
                creature.User = MockHttpContext.CurrentUser;
            }

            creaturesResponse.Should().BeEquivalentTo(creatures.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewCreature_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var creature = await client.CreateCreature();

            var addedCreature = await client.GetEntity<Creature>(creature.Id);
            Validation.CompareEntities(creature, addedCreature);
        }

        [Fact]
        public async Task Post_CreateNewInvalidCreature_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var creature = Generation.Creature();
            creature.Name = null;
            var response = await client.PostAsync("/api/creatures", creature);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_CreatureById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creature = await client.CreateCreature();

            var response = await client.GetAsync($"/api/creatures/{creature.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedCreature = await response.ParseEntity<Creature>();
            Validation.CompareEntities(creature, addedCreature);
        }

        [Fact]
        public async Task Put_CreatureById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creature = await client.CreateCreature();
            var creatureEdit = Generation.Creature();
            creatureEdit.Id = creature.Id;

            var response = await client.PutAsync($"api/creatures/{creature.Id}", creatureEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedCreature = await client.GetEntity<Creature>(creature.Id);
            addedCreature.Should().NotBeNull();
            Validation.CompareEntities(creatureEdit, addedCreature);
        }

        [Fact]
        public async Task Delete_CreatureById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creature = await client.CreateCreature();

            var response = await client.DeleteAsync($"api/creatures/{creature.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var creatureLookup = await client.GetAsync($"api/creatures/{creature.Id}");
            creatureLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
