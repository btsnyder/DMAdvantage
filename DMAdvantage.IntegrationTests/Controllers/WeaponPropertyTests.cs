﻿using TestEngineering.Mocks;
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
using DMAdvantage.Shared.Extensions;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class WeaponPropertyTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public WeaponPropertyTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/{typeof(WeaponProperty).GetPath()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllWeaponProperties_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateWeaponProperty();

            var response = await client.GetAsync($"/api/{typeof(WeaponProperty).GetPath()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponProperties = await response.ParseEntityList<WeaponProperty>();
            var weaponPropertiesFromDb = await client.GetAllEntities<WeaponProperty>();
            weaponProperties.Should().HaveCount(weaponPropertiesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllWeaponPropertiesWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/{typeof(WeaponProperty).GetPath()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponPropertiesResponse = await response.ParseEntityList<WeaponProperty>();

            weaponPropertiesResponse.Should().HaveCount(paging.PageSize);
            weaponPropertiesResponse[0].Id.Should().Be(abilities[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllWeaponPropertiesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/{typeof(WeaponProperty).GetPath()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var weaponPropertiesResponse = await response.ParseEntityList<WeaponProperty>();

            foreach (var weaponProperty in weaponPropertiesResponse)
            {
                weaponProperty.User = MockHttpContext.CurrentUser;
            }

            weaponPropertiesResponse.Should().BeEquivalentTo(weaponProperties.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewWeaponProperty_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var addedWeaponProperty = await client.GetEntity<WeaponProperty>(weaponProperty.Id);
            Validation.CompareEntities(weaponProperty, addedWeaponProperty);
        }

        [Fact]
        public async Task Post_CreateNewInvalidWeaponProperty_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var weaponProperty = Generation.WeaponProperty();
            weaponProperty.Name = null;
            var response = await client.PostAsync($"/api/{typeof(WeaponProperty).GetPath()}", weaponProperty);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_WeaponPropertyById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var response = await client.GetAsync($"/api/{typeof(WeaponProperty).GetPath()}/{weaponProperty.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedWeaponProperty = await response.ParseEntity<WeaponProperty>();
            Validation.CompareEntities(weaponProperty, addedWeaponProperty);
        }

        [Fact]
        public async Task Put_WeaponPropertyById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();
            var weaponPropertyEdit = Generation.WeaponProperty();

            var response = await client.PutAsync($"api/{typeof(WeaponProperty).GetPath()}/{weaponProperty.Id}", weaponPropertyEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedWeaponProperty = await client.GetEntity<WeaponProperty>(weaponProperty.Id);
            addedWeaponProperty.Should().NotBeNull();
            weaponPropertyEdit.Id = weaponProperty.Id;
            Validation.CompareEntities(weaponPropertyEdit, addedWeaponProperty);
        }

        [Fact]
        public async Task Delete_WeaponPropertyById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var weaponProperty = await client.CreateWeaponProperty();

            var response = await client.DeleteAsync($"api/{typeof(WeaponProperty).GetPath()}/{weaponProperty.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var weaponPropertyLookup = await client.GetAsync($"api/{typeof(WeaponProperty).GetPath()}/{weaponProperty.Id}");
            weaponPropertyLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
