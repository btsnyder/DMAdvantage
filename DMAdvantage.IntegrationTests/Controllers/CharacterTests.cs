﻿using TestEngineering.Mocks;
using System.Net;
using System.Threading.Tasks;
using TestEngineering;
using Xunit;
using DMAdvantage.Shared.Models;
using DMAdvantage.Server;
using FluentAssertions;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class CharacterTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public CharacterTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
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
            characters.Should().HaveCount(charactersFromDb.Count);
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var character = await client.CreateCharacter();

            var addedCharacter = await client.GetEntity<CharacterResponse>(character.Id);
            Validation.CompareData(character, addedCharacter);
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
            Validation.CompareData(character, addedCharacter);
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
            addedCharacter.Should().NotBeNull();
            Validation.CompareData(newCharacter, addedCharacter);
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