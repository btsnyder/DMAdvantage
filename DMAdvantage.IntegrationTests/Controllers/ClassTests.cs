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
    public class ClassTests
    {
        private readonly TestServer _server;

        public ClassTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _server.CreateClient();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllClasses_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            await client.CreateDMClass();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classes = await response.ParseEntityList<DMClass>();
            var classesFromDb = await client.GetAllEntities<DMClass>();
            classes.Should().HaveCount(classesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllClassesWithPaging_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var classes = new List<DMClass>();

            for (var i = 0; i < 25; i++)
            {
                var dmclass = Generation.DMClass();
                dmclass.Name = $"{i:00000} - DMClass";
                var dMClass = await client.CreateDMClass(dmclass);
                classes.Add(dMClass);
            }

            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classesResponse = await response.ParseEntityList<DMClass>();

            classesResponse.Should().HaveCount(paging.PageSize);
            classesResponse[0].Id.Should().Be(classes[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllClassesWithSearching_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var classes = new List<DMClass>();

            for (var i = 0; i < 25; i++)
            {
                var dmclass = Generation.DMClass();
                dmclass.Name = $"{i:00000} - DMClass";
                if (Faker.Boolean.Random())
                    dmclass.Name += "Found";
                var dMClass = await client.CreateDMClass(dmclass);
                classes.Add(dMClass);
            }

            var search = new NamedSearchParameters<DMClass>()
            {
                Search = "found"
            };

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classesResponse = await response.ParseEntityList<DMClass>();

            classesResponse.Should().BeEquivalentTo(classes.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewDMClass_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var dmclass = await client.CreateDMClass();

            var addedDMClass = await client.GetEntity<DMClass>(dmclass.Id);
            Validation.CompareEntities(dmclass, addedDMClass);
        }

        [Fact]
        public async Task Post_CreateNewInvalidDMClass_BadRequest()
        {
            var client = await _server.CreateAuthenticatedClientAsync();

            var dmclass = Generation.DMClass();
            dmclass.Name = null;
            var response = await client.PostAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}", dmclass);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_DMClassById_Ok()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();

            var response = await client.GetAsync($"/api/{DMTypeExtensions.GetPath<DMClass>()}/{dmclass.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedDMClass = await response.ParseEntity<DMClass>();
            Validation.CompareEntities(dmclass, addedDMClass);
        }

        [Fact]
        public async Task Put_DMClassById_Created()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();
            var dmclassEdit = Generation.DMClass();
            dmclassEdit.Id = dmclass.Id;

            var response = await client.PutAsync($"api/{DMTypeExtensions.GetPath<DMClass>()}/{dmclass.Id}", dmclassEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedDMClass = await client.GetEntity<DMClass>(dmclass.Id);
            addedDMClass.Should().NotBeNull();
            Validation.CompareEntities(dmclassEdit, addedDMClass);
        }

        [Fact]
        public async Task Delete_DMClassById_NoContent()
        {
            var client = await _server.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();

            var response = await client.DeleteAsync($"api/{DMTypeExtensions.GetPath<DMClass>()}/{dmclass.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var dmclassLookup = await client.GetAsync($"api/{DMTypeExtensions.GetPath<DMClass>()}/{dmclass.Id}");
            dmclassLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
