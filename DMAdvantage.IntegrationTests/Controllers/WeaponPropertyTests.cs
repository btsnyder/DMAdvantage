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

namespace DMAdvantage.UnitTests.Controllers
{
    public class WeaponPropertyTests
    {
        private readonly TestServer _server;

        public WeaponPropertyTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllWeaponProperties_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateWeaponProperty();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponProperties = await response.ParseEntityList<WeaponProperty>();
            var weaponPropertiesFromDb = await client.GetAllEntities<WeaponProperty>();
            weaponProperties.Should().HaveCount(weaponPropertiesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllWeaponPropertiesWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var abilities = new List<WeaponProperty>();

            for (var i = 0; i < 25; i++)
            {
                var weaponProperty = Generation.WeaponProperty();
                weaponProperty.Name = $"{i:00000} - WeaponProperty";
                var weaponPropertyResponse = await client.CreateWeaponProperty(weaponProperty);
                abilities.Add(weaponPropertyResponse);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponPropertiesResponse = await response.ParseEntityList<WeaponProperty>();

            weaponPropertiesResponse.Should().HaveCount(paging.PageSize);
            weaponPropertiesResponse[0].Id.Should().Be(abilities[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllWeaponPropertiesWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weaponProperties = new List<WeaponProperty>();

            for (var i = 0; i < 25; i++)
            {
                var weaponProperty = Generation.WeaponProperty();
                weaponProperty.Name = $"{i:00000} - WeaponProperty";
                if (Faker.Boolean.Random())
                    weaponProperty.Name += "Found";
                var weaponPropertyResponse = await client.CreateWeaponProperty(weaponProperty);
                weaponProperties.Add(weaponPropertyResponse);
            }

            var search = new NamedSearchParameters<WeaponProperty>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponPropertiesResponse = await response.ParseEntityList<WeaponProperty>();

            foreach (var weaponProperty in weaponPropertiesResponse)
            {
                weaponProperty.User = weaponProperties.First().User;
            }

            weaponPropertiesResponse.Should().BeEquivalentTo(weaponProperties.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewWeaponProperty_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var addedWeaponProperty = await client.GetEntity<WeaponProperty>(weaponProperty.Id);
            Validation.CompareEntities(weaponProperty, addedWeaponProperty);
        }

        [Fact]
        public async Task Post_CreateNewInvalidWeaponProperty_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var weaponProperty = Generation.WeaponProperty();
            weaponProperty.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}", weaponProperty);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_WeaponPropertyById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<WeaponProperty>()}/{weaponProperty.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedWeaponProperty = await response.ParseEntity<WeaponProperty>();
            Validation.CompareEntities(weaponProperty, addedWeaponProperty);
        }

        [Fact]
        public async Task Put_WeaponPropertyById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();
            var weaponPropertyEdit = Generation.WeaponProperty();

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<WeaponProperty>()}/{weaponProperty.Id}", weaponPropertyEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedWeaponProperty = await client.GetEntity<WeaponProperty>(weaponProperty.Id);
            addedWeaponProperty.Should().NotBeNull();
            weaponPropertyEdit.Id = weaponProperty.Id;
            Validation.CompareEntities(weaponPropertyEdit, addedWeaponProperty);
        }

        [Fact]
        public async Task Delete_WeaponPropertyById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<WeaponProperty>()}/{weaponProperty.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var weaponPropertyLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<WeaponProperty>()}/{weaponProperty.Id}");
            weaponPropertyLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
