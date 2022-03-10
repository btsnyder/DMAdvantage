using DMAdvantage.Shared.Models;
using FluentAssertions;
using System.Net;
using System.Reflection;
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

        private static string GetPath(MemberInfo t)
        {
            var path = t.Name;
            if (path.Contains("Ability"))
                return "abilities";
            if (path.Contains("Class"))
                return "classes";
            path = path.Replace("Response", "");
            path = path.Replace("Request", "");
            path += "s";
            return path.ToLower();
        }

        public static async Task<CharacterResponse> CreateCharacter(this HttpClient client, CharacterRequest? character = null)
        {
            character ??= Generation.CharacterRequest();
            var ability = await client.CreateAbility();
            character.Abilities.Add(ability);
            var response = await client.PostAsync("/api/characters", character);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<CharacterResponse>();
            return created;
        }

        public static async Task<CreatureResponse> CreateCreature(this HttpClient client, CreatureRequest? creature = null)
        {
            creature ??= Generation.CreatureRequest();
            var response = await client.PostAsync("/api/creatures", creature);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<CreatureResponse>();
            return created;
        }

        public static async Task<EncounterResponse> CreateEncounter(this HttpClient client, EncounterRequest? encounter = null)
        {
            if (encounter == null)
            { 
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

                encounter = new EncounterRequest
                {
                    Data = data,
                };
            }
            var response = await client.PostAsync("/api/encounters", encounter);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<EncounterResponse>();
            return created;
        }

        public static async Task<ForcePowerResponse> CreateForcePower(this HttpClient client, ForcePowerRequest? forcePower = null)
        {
            forcePower ??= Generation.ForcePowerRequest();
            var response = await client.PostAsync("/api/forcepowers", forcePower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<ForcePowerResponse>();
            return created;
        }

        public static async Task<TechPowerResponse> CreateTechPower(this HttpClient client, TechPowerRequest? techPower = null)
        {
            techPower ??= Generation.TechPowerRequest();
            var response = await client.PostAsync("/api/techpowers", techPower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<TechPowerResponse>();
            return created;
        }

        public static async Task<AbilityResponse> CreateAbility(this HttpClient client, AbilityRequest? ability = null)
        {
            ability ??= Generation.AbilityRequest();
            var response = await client.PostAsync("/api/abilities", ability);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<AbilityResponse>();
            return created;
        }

        public static async Task<DMClassResponse> CreateDMClass(this HttpClient client, DMClassRequest? dmclass = null)
        {
            dmclass ??= Generation.DMClassRequest();
            var response = await client.PostAsync("/api/classes", dmclass);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<DMClassResponse>();
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
