using System;
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
    public class CharacterTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public CharacterTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/characters");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllCharacters_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateCharacter();

            var response = await client.GetAsync("/api/characters");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var characters = await response.ParseEntityList<CharacterResponse>();
            var charactersFromDb = await client.GetAllEntities<CharacterResponse>();
            characters.Should().BeEquivalentTo(charactersFromDb);
        }

        [Fact]
        public async Task Get_AllCharactersWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var characters = new List<CharacterResponse>();

            for (var i = 0; i < 25; i++)
            {
                var character = Generation.CharacterRequest();
                character.Name = $"{i:00000} - Character";
                var characterResponse = await client.CreateCharacter(character);
                characters.Add(characterResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/characters?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var charactersResponse = await response.ParseEntityList<CharacterResponse>();

            charactersResponse.Should().HaveCount(paging.PageSize);
            charactersResponse[0].Id.Should().Be(characters[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllCharactersWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var characters = new List<CharacterResponse>();

            for (var i = 0; i < 25; i++)
            {
                var character = Generation.CharacterRequest();
                character.Name = $"{i:00000} - Character";
                if (Faker.Boolean.Random())
                    character.Name += "Found";
                var characterResponse = await client.CreateCharacter(character);
                characters.Add(characterResponse);
            }

            var search = new NamedSearchParameters<Character>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/characters?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var charactersResponse = await response.ParseEntityList<CharacterResponse>();

            charactersResponse.Should().BeEquivalentTo(characters.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var character = await client.CreateCharacter();

            var addedCharacter = await client.GetEntity<CharacterResponse>(character.Id);
            Validation.CompareResponses(character, addedCharacter);
        }

        [Fact]
        public async Task Post_CreateNewInvalidCharacter_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var character = Generation.CharacterRequest();
            character.Name = null;
            var response = await client.PostAsync("/api/characters", character);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_CharacterById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();

            var response = await client.GetAsync($"/api/characters/{character.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedCharacter = await response.ParseEntity<CharacterResponse>();
            Validation.CompareResponses(character, addedCharacter);
        }

        [Fact]
        public async Task Put_CharacterById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();
            var newCharacter = Generation.CharacterRequest();

            var response = await client.PutAsync($"api/characters/{character.Id}", newCharacter);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedCharacter = await client.GetEntity<CharacterResponse>(character.Id);
            Validation.CompareRequests(newCharacter, addedCharacter);
        }

        [Fact]
        public async Task Delete_CharacterById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();

            var response = await client.DeleteAsync($"api/characters/{character.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var characterLookup = await client.GetAsync($"api/characters/{character.Id}");
            characterLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
