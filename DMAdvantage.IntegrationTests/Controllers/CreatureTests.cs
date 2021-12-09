using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Shared.Models;
using DMAdvantage.Server;
using FluentAssertions;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class CreatureTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;
        public CreatureTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
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
            var creatures = await response.ParseEntityList<CreatureResponse>();
            var creaturesFromDb = await client.GetAllEntities<CreatureResponse>();
            creatures.Should().HaveCount(creaturesFromDb.Count);
        }

        [Fact]
        public async Task Post_CreateNewCreature_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var creature = await client.CreateCreature();

            var addedCreature = await client.GetEntity<CreatureResponse>(creature.Id);
            Validation.CompareData(creature, addedCreature);
        }

        [Fact]
        public async Task Post_CreateNewInvalidCreature_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var creature = Generation.CreatureRequest();
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
            var addedCreature = await response.ParseEntity<CreatureResponse>();
            Validation.CompareData(creature, addedCreature);
        }

        [Fact]
        public async Task Put_CreatureById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var creature = await client.CreateCreature();
            var creatureEdit = Generation.CreatureRequest();

            var response = await client.PutAsync($"api/creatures/{creature.Id}", creatureEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedCreature = await client.GetEntity<CreatureResponse>(creature.Id);
            addedCreature.Should().NotBeNull();
            Validation.CompareData(creatureEdit, addedCreature);
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
