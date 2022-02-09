using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Server.Controllers;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.UnitTests.Controllers
{
    public class AccountTests
    {
        private readonly Dictionary<string, string> _mockConfiguration = new()
        {
            { "Tokens:Key", "a;sdlkfja; lsdkfj ;alksdfj ;alksdfj; aiefj;lskij;fldsk" },
            { "Tokens:Issuer", "localhost" },
            { "Tokens:Audience", "localhost" }
        };

        private readonly MockLogger<AccountController> _mockLogger;
        private readonly AccountController _mockController;

        public AccountTests()
        {
            _mockLogger = new MockLogger<AccountController>();
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);
            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(_mockConfiguration)
               .Build();

            _mockController = new AccountController(
                _mockLogger,
                MockSigninManagerFactory.Create(),
                MockUserManagerFactory.Create(),
                configuration,
                mapper);

            _mockController.ControllerContext.HttpContext = httpContextMock;
        }

        [Fact]
        public async Task Post_CreateToken_Success()
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser.UserName,
                Password = MockSigninManagerFactory.CurrentPassword,
            };

            var result = await _mockController.CreateToken(login);

            var created = result.Should().BeOfType<CreatedResult>();
            var response = created.Subject.Value.Should().BeOfType<LoginResponse>();
            response.Subject.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Post_CreateToken_Failure()
        {
            var login = new LoginRequest
            {
                Username = MockHttpContext.CurrentUser.UserName,
                Password = "badpassword",
            };

            var result = await _mockController.CreateToken(login);

            result.Should().BeOfType<BadRequestResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

    }
}
