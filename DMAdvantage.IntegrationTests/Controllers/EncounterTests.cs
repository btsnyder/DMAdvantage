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
            var encounters = await response.ParseEntityList<EncounterResponse>();
            var encountersFromDb = await client.GetAllEntities<EncounterResponse>();
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

                var encounter = new EncounterRequest
                {
                    Data = data,
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
            var encountersResponse = await response.ParseEntityList<EncounterResponse>();

            encountersResponse.Should().HaveCount(paging.PageSize);
        }

        [Fact]
        public async Task Get_AllCreaturesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var encounters = new List<EncounterResponse>();

            for (var i = 0; i < 25; i++)
            {
                var encounter = Generation.EncounterRequest();
                encounter.Name = $"{i:00000} - Encounter";
                if (Faker.Boolean.Random())
                    encounter.Name += "Found";
                var encounterResponse = await client.CreateEncounter(encounter);
                encounters.Add(encounterResponse);
            }

            var search = new NamedSearchParameters<Encounter>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/encounters?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var encounterResponses = await response.ParseEntityList<EncounterResponse>();

            var expected = encounters.Where(x => x.Name?.ToLower().Contains("found") == true);
            encounterResponses.Should().BeEquivalentTo(expected);
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
            Validation.CompareRequests(encounter, addedEncounter);
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

            var encounterEdit = new EncounterRequest
            {
                Data = data,
            };
            var response = await client.PutAsync($"api/encounters/{encounter.Id}", encounterEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedEncounter = await client.GetEntity<EncounterResponse>(encounter.Id);
            addedEncounter.Should().NotBeNull();
            Validation.CompareRequests(encounterEdit, addedEncounter);
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
