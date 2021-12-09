using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Server;
using DMAdvantage.Shared.Models;
using System.Collections.Generic;
using System;
using FluentAssertions;
using System.Linq;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class EncounterTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public EncounterTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/encounters");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllEncounters_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            await client.CreateEncounter();

            var response = await client.GetAsync("/api/encounters");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encounters = await response.ParseEntityList<EncounterResponse>();
            var encountersFromDb = await client.GetAllEntities<EncounterResponse>();
            encounters.Should().HaveCount(encountersFromDb.Count);
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var encounters = await client.GetAllEntities<EncounterResponse>();
            encounters.Where(e => e.Id == encounter.Id).Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_EncounterById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var response = await client.GetAsync($"/api/encounters/{encounter.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedEncounter = await response.ParseEntity<EncounterResponse>();
            Validation.CompareData(encounter, addedEncounter);
        }

        [Fact]
        public async Task Put_EncounterById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var encounter = await client.CreateEncounter();

            var characters = new List<Guid>();
            for (int i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var character = await client.CreateCharacter();
                characters.Add(character.Id);
            }

            var creatures = new List<Guid>();
            for (int i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var creature = await client.CreateCreature();
                creatures.Add(creature.Id);
            }

            var encounterEdit = new EncounterRequest
            {
                CharacterIds = characters,
                CreatureIds = creatures
            };
            var response = await client.PutAsync($"api/encounters/{encounter.Id}", encounterEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedEncounter = await client.GetEntity<EncounterResponse>(encounter.Id);
            addedEncounter.Should().NotBeNull();
            Validation.CompareData(encounterEdit, addedEncounter);
        }

        [Fact]
        public async Task Delete_EncounterById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var encounter = await client.CreateEncounter();

            var response = await client.DeleteAsync($"api/encounters/{encounter.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var encounterLookup = await client.GetAsync($"api/encounters/{encounter.Id}");
            encounterLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
