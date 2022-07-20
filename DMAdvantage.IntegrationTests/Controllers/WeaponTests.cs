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
    public class WeaponTests
    {
        private readonly TestServer _server;

        public WeaponTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllWeapons_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateWeapon();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weapons = await response.ParseEntityList<Weapon>();
            var weaponsFromDb = await client.GetAllEntities<Weapon>();
            weapons.Should().HaveCount(weaponsFromDb.Count);
        }

        [Fact]
        public async Task Get_AllWeaponsWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weapons = new List<Weapon>();

            for (var i = 0; i < 25; i++)
            {
                var weapon = Generation.Weapon();
                weapon.Name = $"{i:00000} - Weapon";
                var weaponResponse = await client.CreateWeapon(weapon);
                weapons.Add(weaponResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponsResponse = await response.ParseEntityList<Weapon>();

            weaponsResponse.Should().HaveCount(paging.PageSize);
            weaponsResponse[0].Id.Should().Be(weapons[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllWeaponsWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weapons = new List<Weapon>();

            for (var i = 0; i < 25; i++)
            {
                var weapon = Generation.Weapon();
                weapon.Name = $"{i:00000} - Weapon";
                if (Faker.Boolean.Random())
                    weapon.Name += "Found";
                var weaponResponse = await client.CreateWeapon(weapon);
                weapons.Add(weaponResponse);
            }

            var search = new NamedSearchParameters<Weapon>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponsResponse = await response.ParseEntityList<Weapon>();

            foreach (var weapon in weaponsResponse)
            {
                weapon.User = weapons.First().User;
            }

            weaponsResponse.Should().BeEquivalentTo(weapons.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewWeapon_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var weapon = await client.CreateWeapon();

            var addedWeapon = await client.GetEntity<Weapon>(weapon.Id);
            Validation.CompareEntities(weapon, addedWeapon);
        }

        [Fact]
        public async Task Post_CreateNewInvalidWeapon_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var weapon = Generation.Weapon();
            weapon.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}", weapon);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_WeaponById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weapon = await client.CreateWeapon();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<Weapon>()}/{weapon.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedWeapon = await response.ParseEntity<Weapon>();
            Validation.CompareEntities(weapon, addedWeapon);
        }

        [Fact]
        public async Task Put_WeaponById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weapon = await client.CreateWeapon();
            var weaponEdit = Generation.Weapon();
            weaponEdit.Id = weapon.Id;

            weaponEdit.Properties = weapon.Properties;
            var newProperty = await client.CreateWeaponProperty();
            newProperty.User = null;
            weaponEdit.Properties.Add(newProperty);

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<Weapon>()}/{weapon.Id}", weaponEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedWeapon = await client.GetEntity<Weapon>(weapon.Id);
            addedWeapon.Should().NotBeNull();
            Validation.CompareEntities(weaponEdit, addedWeapon);
        }

        [Fact]
        public async Task Delete_WeaponById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weapon = await client.CreateWeapon();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<Weapon>()}/{weapon.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var weaponLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<Weapon>()}/{weapon.Id}");
            weaponLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
