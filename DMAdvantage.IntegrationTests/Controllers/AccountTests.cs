﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TestEngineering;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class AccountTests
    {
        private readonly TestServer _server;

        public AccountTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Post_CreateToken_Success()
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser,
                Password = MockSigninManagerFactory.CurrentPassword,
            };
            var client = _server.CreateClient();

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
                Username = MockHttpContext.CurrentUser,
                Password = "badpassword",
            };
            var client = _server.CreateClient();

            var response = await client.PostAsync("/api/account/token", login);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
