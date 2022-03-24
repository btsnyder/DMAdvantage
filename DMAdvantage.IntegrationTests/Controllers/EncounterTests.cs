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
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using System.Text.Json;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class EncounterTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public EncounterTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
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
            var encounters = await response.ParseEntityList<Encounter>();
            var encountersFromDb = await client.GetAllEntities<Encounter>();
            encounters.Should().HaveCount(encountersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllEncountersWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            for (var i = 0; i < 25; i++)
            {
                var characters = new List<Guid>();
                for (var j = 0; j < Faker.RandomNumber.Next(1, 5); j++)
                {
                    var character = await client.CreateCharacter();
                    characters.Add(character.Id);
                }

                var creatures = new List<Guid>();
                for (var j = 0; j < Faker.RandomNumber.Next(1, 5); j++)
                {
                    var creature = await client.CreateCreature();
                    creatures.Add(creature.Id);
                }

                var data = new List<InitativeData>();
                data.AddRange(characters.Select(x => new InitativeData { BeingId = x }));
                data.AddRange(creatures.Select(x => new InitativeData { BeingId = x }));

                var encounter = new Encounter
                {
                    DataCache = JsonSerializer.Serialize(data),
                    ConcentrationCache = JsonSerializer.Serialize(new Dictionary<string, Guid>())
                };

                await client.CreateEncounter(encounter);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/encounters?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encountersResponse = await response.ParseEntityList<Encounter>();

            encountersResponse.Should().HaveCount(paging.PageSize);
        }

        [Fact]
        public async Task Get_AllCreaturesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var encounters = new List<Encounter>();

            for (var i = 0; i < 25; i++)
            {
                var encounter = Generation.Encounter();
                encounter.Name = $"{i:00000} - Encounter";
                if (Faker.Boolean.Random())
                    encounter.Name += "Found";
                var addedEncounter = await client.CreateEncounter(encounter);
                encounters.Add(addedEncounter);
            }

            var search = new NamedSearchParameters<Encounter>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/encounters?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encoutnersResponse = await response.ParseEntityList<Encounter>();

            foreach (var encounter in encoutnersResponse)
            {
                encounter.User = MockHttpContext.CurrentUser;
            }

            var expected = encounters.Where(x => x.Name?.ToLower().Contains("found") == true);
            encoutnersResponse.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var encounters = await client.GetAllEntities<Encounter>();
            encounters.Where(e => e.Id == encounter.Id).Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_EncounterById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var response = await client.GetAsync($"/api/encounters/{encounter.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedEncounter = await response.ParseEntity<Encounter>();
            Validation.CompareEntities(encounter, addedEncounter);
        }

        [Fact]
        public async Task Put_EncounterById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var encounter = await client.CreateEncounter();

            var characters = new List<Guid>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var character = await client.CreateCharacter();
                characters.Add(character.Id);
            }

            var creatures = new List<Guid>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var creature = await client.CreateCreature();
                creatures.Add(creature.Id);
            }

            var data = new List<InitativeData>();
            data.AddRange(characters.Select(x => new InitativeData { BeingId = x }));
            data.AddRange(creatures.Select(x => new InitativeData { BeingId = x }));

            var encounterEdit = new Encounter
            {
                DataCache = JsonSerializer.Serialize(data),
                ConcentrationCache = JsonSerializer.Serialize(new Dictionary<string, Guid>())
            };
            encounterEdit.Id = encounter.Id;
            var response = await client.PutAsync($"api/encounters/{encounter.Id}", encounterEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedEncounter = await client.GetEntity<Encounter>(encounter.Id);
            addedEncounter.Should().NotBeNull();
            Validation.CompareEntities(encounterEdit, addedEncounter);
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
