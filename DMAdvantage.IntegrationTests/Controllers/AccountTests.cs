using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using TestEngineering.Mocks;
using DMAdvantage.Shared.Models;
using TestEngineering;
using DMAdvantage.Server;
using FluentAssertions;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class AccountTests : IClassFixture<MockWebApplicationFactory<Startup>>
    {
        private readonly MockWebApplicationFactory<Startup> _factory;

        public AccountTests(MockWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_CreateToken_Success()
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser.UserName,
                Password = MockSigninManagerFactory.CurrentPassword,
            };
            var client = _factory.CreateClient();

            var response = await client.PostAsync("/api/account/token", login);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.ParseEntity<Dictionary<string, string>>();
            content["token"].Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Post_CreateToken_Failure()
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser.UserName,
                Password = "badpassword",
            };
            var client = _factory.CreateClient();

            var response = await client.PostAsync("/api/account/token", login);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
