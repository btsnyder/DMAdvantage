using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Controllers
{
    public class EncounterTests
    {
        private readonly TestServer _server;

        public EncounterTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Encounter>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllEncounters_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            await client.CreateEncounter();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Encounter>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encounters = await response.ParseEntityList<Encounter>();
            var encountersFromDb = await client.GetAllEntities<Encounter>();
            encounters.Should().HaveCount(encountersFromDb.Count);
        }

        [Fact]
        public async Task Get_AllEncountersWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            for (var i = 0; i < 25; i++)
            {
                await client.CreateEncounter();
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Encounter>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encountersResponse = await response.ParseEntityList<Encounter>();

            encountersResponse.Should().HaveCount(paging.PageSize);
        }

        [Fact]
        public async Task Get_AllEncountersWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Encounter>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encoutnersResponse = await response.ParseEntityList<Encounter>();

            foreach (var encounter in encoutnersResponse)
            {
                encounter.User = encounters.First().User;
            }

            var expected = encounters.Where(x => x.Name?.ToLower().Contains("found") == true);
            encoutnersResponse.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var addedEncounter = await client.GetEntity<Encounter>(encounter.Id);
            addedEncounter.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_EncounterById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var encounter = await client.CreateEncounter();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Encounter>()}/{encounter.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedEncounter = await response.ParseEntity<Encounter>();
            Validation.CompareEntities(encounter, addedEncounter);
        }

        [Fact]
        public async Task Put_EncounterById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var encounter = await client.CreateEncounter();

            var characters = new List<Character>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var character = await client.CreateCharacter();
                character.User = null;
                characters.Add(character);
            }

            var creatures = new List<Creature>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, 5); i++)
            {
                var creature = await client.CreateCreature();
                creature.User = null;
                creatures.Add(creature);
                creatures.Add(creature);
            }

            var data = new List<InitativeData>();
            data.AddRange(characters.Select(x => new InitativeData { Character = x, CharacterId = x.Id }));
            data.AddRange(creatures.Select(x => new InitativeData { Creature = x, CreatureId = x.Id }));

            var encounterEdit = new Encounter
            {
                InitativeData = data,
                ConcentrationCache = JsonSerializer.Serialize(new Dictionary<string, Guid>())
            };
            encounterEdit.Id = encounter.Id;
            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<Encounter>()}/{encounter.Id}", encounterEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedEncounter = await client.GetEntity<Encounter>(encounter.Id);
            addedEncounter.Should().NotBeNull();

            for (int i = 0; i < encounterEdit.InitativeData.Count; i++)
            {
                encounterEdit.InitativeData.ElementAt(i).Id = addedEncounter.InitativeData.ElementAt(i).Id;
            }

            Validation.CompareEntities(encounterEdit, addedEncounter);
        }

        [Fact]
        public async Task Delete_EncounterById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var encounter = await client.CreateEncounter();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<Encounter>()}/{encounter.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var encounterLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<Encounter>()}/{encounter.Id}");
            encounterLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
