using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
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
            var response = await client.GetAsync($"/api/{typeof(T).GetPath()}/{id}");
            var entity = await response.ParseEntity<T>();
            return entity;
        }

        public static async Task<List<T>> GetAllEntities<T>(this HttpClient client)
        {
            var response = await client.GetAsync($"/api/{typeof(T).GetPath()}");
            var entities = await response.ParseEntityList<T>();
            return entities;
        }

        public static async Task<Character> CreateCharacter(this HttpClient client, Character? character = null)
        {
            var random = new Random();
            character ??= Generation.Character();
            character.User = null;
            var ability = await client.CreateAbility();
            ability.User = null;
            character.Abilities.Add(ability);
            var powers = new List<ForcePower>();
            for (int i = 0; i < random.Next(0, 10); i++)
            {
                var power = await client.CreateForcePower();
                power.User = null;
                powers.Add(power);
            }
            character.ForcePowers = powers;
            var weapon = await client.CreateWeapon();
            weapon.Properties = null;
            weapon.User = null;
            character.Weapons.Add(weapon);
            var response = await client.PostAsync($"/api/{typeof(Character).GetPath()}", character);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<Character>();
            return created;
        }

        public static async Task<Creature> CreateCreature(this HttpClient client, Creature? creature = null)
        {
            var random = new Random();
            creature ??= Generation.Creature();
            creature.User = null;
            var powers = new List<ForcePower>();
            for (int i = 0; i < random.Next(0, 10); i++)
            {
                var power = await client.CreateForcePower();
                power.User = null;
                powers.Add(power);
            }
            creature.ForcePowers = powers;

            var actions = new List<BaseAction>();
            for (int i = 0; i < random.Next(0, 10); i++)
            {
                var action = Generation.BaseAction();
                actions.Add(action);
            }
            creature.Actions = actions;

            var response = await client.PostAsync($"/api/{typeof(Creature).GetPath()}", creature);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<Creature>();
            return created;
        }

        public static async Task<Encounter> CreateEncounter(this HttpClient client, Encounter? encounter = null)
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

                encounter = new Encounter
                {
                    Name = Faker.Name.FullName(),
                    DataCache = JsonSerializer.Serialize(data),
                    ConcentrationCache = JsonSerializer.Serialize(new Dictionary<string, Guid>())
                };
            }
            var response = await client.PostAsync($"/api/{typeof(Encounter).GetPath()}", encounter);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<Encounter>();
            return created;
        }

        public static async Task<ForcePower> CreateForcePower(this HttpClient client, ForcePower? forcePower = null)
        {
            forcePower ??= Generation.ForcePower();
            var response = await client.PostAsync($"/api/{typeof(ForcePower).GetPath()}", forcePower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<ForcePower>();
            return created;
        }

        public static async Task<TechPower> CreateTechPower(this HttpClient client, TechPower? techPower = null)
        {
            techPower ??= Generation.TechPower();
            var response = await client.PostAsync($"/api/{typeof(TechPower).GetPath()}", techPower);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<TechPower>();
            return created;
        }

        public static async Task<Ability> CreateAbility(this HttpClient client, Ability? ability = null)
        {
            ability ??= Generation.Ability();
            var response = await client.PostAsync($"/api/{typeof(Ability).GetPath()}", ability);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<Ability>();
            return created;
        }

        public static async Task<DMClass> CreateDMClass(this HttpClient client, DMClass? dmclass = null)
        {
            dmclass ??= Generation.DMClass();
            var response = await client.PostAsync($"/api/{typeof(DMClass).GetPath()}", dmclass);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<DMClass>();
            return created;
        }

        public static async Task<Weapon> CreateWeapon(this HttpClient client, Weapon? weapon = null)
        {
            var random = new Random();
            weapon ??= Generation.Weapon();
            weapon.User = null;
            var properties = new List<WeaponProperty>();
            for (int i = 0; i < random.Next(0, 10); i++)
            {
                var prop = await client.CreateWeaponProperty();
                prop.User = null;
                properties.Add(prop);
            }
            weapon.Properties = properties;
            var response = await client.PostAsync($"/api/{typeof(Weapon).GetPath()}", weapon);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<Weapon>();
            return created;
        }

        public static async Task<WeaponProperty> CreateWeaponProperty(this HttpClient client, WeaponProperty? property = null)
        {
            property ??= Generation.WeaponProperty();
            var response = await client.PostAsync($"/api/{typeof(WeaponProperty).GetPath()}", property);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.ParseEntity<WeaponProperty>();
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
