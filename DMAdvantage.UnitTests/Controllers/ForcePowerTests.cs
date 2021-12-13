using AutoMapper;
using TestEngineering;
using TestEngineering.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Server.Controllers;
using DMAdvantage.Data;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.UnitTests.Controllers
{
    public class ForcePowerTests
    {
        readonly List<ForcePower> _mockForcePowerData = new()
        {
            Generation.ForcePower()
        };
        readonly MockLogger<ForcePowersController> _mockLogger;

        public ForcePowerTests()
        {
            _mockLogger = new MockLogger<ForcePowersController>();
        }

        ForcePowersController CreateMockForcePowerController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var forcePowerController = new ForcePowersController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            forcePowerController.ControllerContext.HttpContext = httpContextMock;
            return forcePowerController;
        }

        [Fact]
        public void Get_AllForcePowers_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<ForcePower>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Returns(_mockForcePowerData);
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = forcePowerController.GetAllForcePowers();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<ForcePowerResponse>>();
            response.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockForcePowerData[0], response.Subject[0]);
        }

        [Fact]
        public void Get_ForcePowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<ForcePower>(_mockForcePowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockForcePowerData[0]);
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = forcePowerController.GetForcePowerById(_mockForcePowerData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<ForcePowerResponse>();
            Validation.CompareData(_mockForcePowerData[0], response.Subject);
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockForcePowerData.Add((ForcePower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newForcePower = Generation.ForcePowerRequest();
            var originalCount = _mockForcePowerData.Count;
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = await forcePowerController.CreateNewForcePower(newForcePower);

            result.Should().BeOfType<CreatedResult>();
            _mockForcePowerData.Should().HaveCount(originalCount + 1);
            var addedForcePower = _mockForcePowerData.Last();
            Validation.CompareData(newForcePower, addedForcePower);
        }

        [Fact]
        public async Task Put_UpdateForcePowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<ForcePower>(_mockForcePowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockForcePowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editForcePower = new ForcePowerRequest
            {
                Name = Generation.Nonsense(),
                Description = _mockForcePowerData[0].Description,
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = _mockForcePowerData[0].CastingPeriod,
                CastingDescription = _mockForcePowerData[0].CastingDescription,
                Range = Generation.RandomEnum<PowerRange>(),
                RangeDescription = Generation.Nonsense(50),
                Duration = _mockForcePowerData[0].Duration,
                Concentration = Faker.Boolean.Random(),
                HitOption = _mockForcePowerData[0].HitOption,
                HitDescription = Generation.Nonsense(50),
                Alignment = _mockForcePowerData[0].Alignment,
                Potency = Generation.Nonsense()
            };
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = await forcePowerController.UpdateForcePowerById(_mockForcePowerData[0].Id, editForcePower);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editForcePower, _mockForcePowerData[0]);
        }

        [Fact]
        public async Task Put_NewForcePowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockForcePowerData.Add((ForcePower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editForcePower = new ForcePowerRequest
            {
                Name = Generation.Nonsense(),
                Description = _mockForcePowerData[0].Description,
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = _mockForcePowerData[0].CastingPeriod,
                CastingDescription = _mockForcePowerData[0].CastingDescription,
                Range = Generation.RandomEnum<PowerRange>(),
                RangeDescription = Generation.Nonsense(50),
                Duration = _mockForcePowerData[0].Duration,
                Concentration = Faker.Boolean.Random(),
                HitOption = _mockForcePowerData[0].HitOption,
                HitDescription = Generation.Nonsense(50),
                Alignment = _mockForcePowerData[0].Alignment,
                Potency = Generation.Nonsense()
            };
            var originalCount = _mockForcePowerData.Count;
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = await forcePowerController.UpdateForcePowerById(Guid.NewGuid(), editForcePower);

            result.Should().BeOfType<CreatedResult>();
            _mockForcePowerData.Should().HaveCount(originalCount + 1);
            var addedForcePower = _mockForcePowerData.Last();
            Validation.CompareData(editForcePower, addedForcePower);
        }


        [Fact]
        public void Get_AllForcePowers_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<ForcePower>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Throws(new Exception());
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = forcePowerController.GetAllForcePowers();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_ForcePowerById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<ForcePower>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = forcePowerController.GetForcePowerById(_mockForcePowerData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = await forcePowerController.CreateNewForcePower(new ForcePowerRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateForcePowerById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = await forcePowerController.UpdateForcePowerById(_mockForcePowerData[0].Id, new ForcePowerRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllForcePowersWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<ForcePower>(It.IsAny<string>(), It.IsAny<PagingParameters?>()))
                .Returns(_mockForcePowerData);
            mockRepo.Setup(x => x.GetAllEntities<ForcePower>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Returns(new List<ForcePower>());
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);

            var result = forcePowerController.GetAllForcePowers();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<ForcePowerResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_ForcePowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<ForcePower>(_mockForcePowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockForcePowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockForcePowerData.Remove((ForcePower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var forcePower = _mockForcePowerData[0];
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);
            var result = forcePowerController.DeleteForcePowerById(forcePower.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockForcePowerData.Should().NotContain(forcePower);
        }

        [Fact]
        public void Delete_ForcePowerByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<ForcePower>(_mockForcePowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockForcePowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockForcePowerData.Remove((ForcePower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var forcePowerController = CreateMockForcePowerController(mockRepo.Object);
            var result = forcePowerController.DeleteForcePowerById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
