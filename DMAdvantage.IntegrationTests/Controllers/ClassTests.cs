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
    public class ClassTests
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public ClassTests()
        {
            _factory = new MockWebApplicationFactory<Startup>();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/classes");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_AllClasses_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            await client.CreateDMClass();

            var response = await client.GetAsync("/api/classes");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classes = await response.ParseEntityList<DMClass>();
            var classesFromDb = await client.GetAllEntities<DMClass>();
            classes.Should().HaveCount(classesFromDb.Count);
        }

        [Fact]
        public async Task Get_AllClassesWithPaging_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/classes?pageSize={paging.PageSize}&pageNumber={paging.PageNumber}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classesResponse = await response.ParseEntityList<DMClass>();

            classesResponse.Should().HaveCount(paging.PageSize);
            classesResponse[0].Id.Should().Be(classes[0 + paging.PageSize].Id);
        }

        [Fact]
        public async Task Get_AllClassesWithSearching_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
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

            var response = await client.GetAsync($"/api/classes?search={search.Search}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var classesResponse = await response.ParseEntityList<DMClass>();

            classesResponse.Should().BeEquivalentTo(classes.Where(x => x.Name?.ToLower().Contains("found") == true));
        }

        [Fact]
        public async Task Post_CreateNewDMClass_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var dmclass = await client.CreateDMClass();

            var addedDMClass = await client.GetEntity<DMClass>(dmclass.Id);
            Validation.CompareEntities(dmclass, addedDMClass);
        }

        [Fact]
        public async Task Post_CreateNewInvalidDMClass_BadRequest()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var dmclass = Generation.DMClass();
            dmclass.Name = null;
            var response = await client.PostAsync("/api/classes", dmclass);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Get_DMClassById_Ok()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();

            var response = await client.GetAsync($"/api/classes/{dmclass.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var addedDMClass = await response.ParseEntity<DMClass>();
            Validation.CompareEntities(dmclass, addedDMClass);
        }

        [Fact]
        public async Task Put_DMClassById_Created()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();
            var dmclassEdit = Generation.DMClass();
            dmclassEdit.Id = dmclass.Id;

            var response = await client.PutAsync($"api/classes/{dmclass.Id}", dmclassEdit);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var addedDMClass = await client.GetEntity<DMClass>(dmclass.Id);
            addedDMClass.Should().NotBeNull();
            Validation.CompareEntities(dmclassEdit, addedDMClass);
        }

        [Fact]
        public async Task Delete_DMClassById_NoContent()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var dmclass = await client.CreateDMClass();

            var response = await client.DeleteAsync($"api/classes/{dmclass.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var dmclassLookup = await client.GetAsync($"api/classes/{dmclass.Id}");
            dmclassLookup.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
