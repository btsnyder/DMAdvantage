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
    public class CharacterTests
    {
        private readonly TestServer _server;

        public CharacterTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Character>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllCharacters_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateCharacter();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Character>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var characters = await response.ParseEntityList<Character>();
            var charactersFromDb = await client.GetAllEntities<Character>();
            characters.Should().BeEquivalentTo(charactersFromDb);
        }

        [Fact]
        public async Task Get_AllCharactersWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var characters = new List<Character>();

            for (var i = 0; i < 25; i++)
            {
                var character = Generation.Character();
                character.Name = $"{i:00000} - Character";
                var characterResponse = await client.CreateCharacter(character);
                characters.Add(characterResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Character>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var charactersResponse = await response.ParseEntityList<Character>();

            charactersResponse.Should().HaveCount(paging.PageSize);
            charactersResponse[0].Id.Should().Be(characters[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllCharactersWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var characters = new List<Character>();

            for (var i = 0; i < 25; i++)
            {
                var character = Generation.Character();
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

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Character>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var charactersResponse = await response.ParseEntityList<Character>();

            foreach (var character in charactersResponse)
            {
                character.User = characters.First().User;
            }

            charactersResponse.Should().BeEquivalentTo(characters.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var character = await client.CreateCharacter();

            var addedCharacter = await client.GetEntity<Character>(character.Id);
            Validation.CompareEntities(character, addedCharacter);
        }

        [Fact]
        public async Task Post_CreateNewInvalidCharacter_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var character = Generation.Character();
            character.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<Character>()}", character);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_CharacterById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Character>()}/{character.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedCharacter = await response.ParseEntity<Character>();
            Validation.CompareEntities(character, addedCharacter);
        }

        [Fact]
        public async Task Put_CharacterById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();
            var newCharacter = Generation.Character();
            newCharacter.Id = character.Id;
            newCharacter.User = null;
            newCharacter.Abilities = character.Abilities;
            var newAbility = await client.CreateAbility();
            newAbility.User = null;
            newAbility.Characters = new List<Character>();
            newCharacter.Abilities.Add(newAbility);
            newCharacter.Weapons = character.Weapons;
            var newWeapon = await client.CreateWeapon();
            newWeapon.User = null;
            newCharacter.Weapons.Add(newWeapon);

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<Character>()}/{character.Id}", newCharacter);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedCharacter = await client.GetEntity<Character>(character.Id);
            Validation.CompareEntities(newCharacter, addedCharacter);
        }

        [Fact]
        public async Task Delete_CharacterById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var character = await client.CreateCharacter();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<Character>()}/{character.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var characterLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<Character>()}/{character.Id}");
            characterLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
