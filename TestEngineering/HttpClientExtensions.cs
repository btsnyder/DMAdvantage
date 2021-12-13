using DMAdvantage.Shared.Models;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using TestEngineering.Mocks;

namespace TestEngineering
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string path, T obj)
        {
            var content = ConvertToContent(obj);
            var response = await client.PostAsync(path, content);
            return response;
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(this HttpClient client, string path, T obj)
        {
            var content = ConvertToContent(obj);
            var response = await client.PutAsync(path, content);
            return response;
        }

        private static StringContent ConvertToContent<T>(T entity)
        {
            return new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
        }

        public static async Task<T> GetEntity<T>(this HttpClient client, Guid id)
        {
            var response = await client.GetAsync($"/api/{GetPath(typeof(T))}/{id}");
            var entity = await response.ParseEntity<T>();
            return entity;
        }

        public static async Task<List<T>> GetAllEntities<T>(this HttpClient client)
        {
            var response = await client.GetAsync($"/api/{GetPath(typeof(T))}");
            var entities = await response.ParseEntityList<T>();
            return entities;
        }

        private static string GetPath(Type t)
        {
            var path = t.Name;
            path = path.Replace("Response", "");
            path = path.Replace("Request", "");
            path += "s";
            return path.ToLower();
        }

        public static async Task<CharacterResponse> CreateCharacter(this HttpClient client)
        {
            var character = Generation.CharacterRequest();
            var response = await client.PostAsync("/api/characters", character);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<CharacterResponse>();
            return created;
        }

        public static async Task<CreatureResponse> CreateCreature(this HttpClient client)
        {
            var part = Generation.CreatureRequest();
            var response = await client.PostAsync("/api/creatures", part);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<CreatureResponse>();
            return created;
        }

        public static async Task<EncounterResponse> CreateEncounter(this HttpClient client)
        {
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

            var encounter = new EncounterRequest
            {
                CharacterIds = characters,
                CreatureIds = creatures
            };
            var response = await client.PostAsync("/api/encounters", encounter);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<EncounterResponse>();
            return created;
        }

        public static async Task<ForcePowerResponse> CreateForcePower(this HttpClient client)
        {
            var forcePower = Generation.ForcePowerRequest();
            var response = await client.PostAsync("/api/forcepowers", forcePower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<ForcePowerResponse>();
            return created;
        }

        public static async Task<TechPowerResponse> CreateTechPower(this HttpClient client)
        {
            var techPower = Generation.ForcePowerRequest();
            var response = await client.PostAsync("/api/techpowers", techPower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<TechPowerResponse>();
            return created;
        }

        public static async Task<DamageTypeResponse> CreateDamageType(this HttpClient client)
        {
            var damageType = Generation.DamageTypeRequest();
            var response = await client.PostAsync("/api/damagetypes", damageType);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<DamageTypeResponse>();
            return created;
        }

        public static async Task<string> CreateToken(this HttpClient client)
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser.UserName,
                Password = MockSigninManagerFactory.CurrentPassword,
            };

            var response = await client.PostAsync("/api/account/token", login);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.ParseEntity<Dictionary<string, string>>();
            return content["token"];
        }
    }
}
