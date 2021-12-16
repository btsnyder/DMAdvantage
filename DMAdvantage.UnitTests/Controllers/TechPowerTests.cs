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
    public class TechPowerTests
    {
        readonly List<TechPower> _mockTechPowerData = new()
        {
            Generation.TechPower()
        };
        readonly MockLogger<TechPowersController> _mockLogger;

        public TechPowerTests()
        {
            _mockLogger = new MockLogger<TechPowersController>();
        }

        TechPowersController CreateMockTechPowerController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var techPowerController = new TechPowersController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            techPowerController.ControllerContext.HttpContext = httpContextMock;
            return techPowerController;
        }

        [Fact]
        public void Get_AllTechPowers_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<TechPower>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(_mockTechPowerData);
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = techPowerController.GetAllTechPowers();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<TechPowerResponse>>();
            response.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockTechPowerData[0], response.Subject[0]);
        }

        [Fact]
        public void Get_TechPowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<TechPower>(_mockTechPowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockTechPowerData[0]);
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = techPowerController.GetTechPowerById(_mockTechPowerData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<TechPowerResponse>();
            Validation.CompareData(_mockTechPowerData[0], response.Subject);
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockTechPowerData.Add((TechPower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newTechPower = Generation.TechPowerRequest();
            var originalCount = _mockTechPowerData.Count;
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = await techPowerController.CreateNewTechPower(newTechPower);

            result.Should().BeOfType<CreatedResult>();
            _mockTechPowerData.Should().HaveCount(originalCount + 1);
            var addedTechPower = _mockTechPowerData.Last();
            Validation.CompareData(newTechPower, addedTechPower);
        }

        [Fact]
        public async Task Put_UpdateTechPowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<TechPower>(_mockTechPowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockTechPowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editTechPower = new TechPowerRequest
            {
                Name = Generation.Nonsense(),
                Description = _mockTechPowerData[0].Description,
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = _mockTechPowerData[0].CastingPeriod,
                CastingDescription = _mockTechPowerData[0].CastingDescription,
                Range = Generation.RandomEnum<PowerRange>(),
                RangeDescription = Generation.Nonsense(50),
                Duration = _mockTechPowerData[0].Duration,
                Concentration = Faker.Boolean.Random(),
                HitOption = _mockTechPowerData[0].HitOption,
                HitDescription = Generation.Nonsense(50),
                Overcharge = _mockTechPowerData[0].Overcharge
            };
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = await techPowerController.UpdateTechPowerById(_mockTechPowerData[0].Id, editTechPower);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editTechPower, _mockTechPowerData[0]);
        }

        [Fact]
        public async Task Put_NewTechPowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockTechPowerData.Add((TechPower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editTechPower = new TechPowerRequest
            {
                Name = Generation.Nonsense(),
                Description = _mockTechPowerData[0].Description,
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = _mockTechPowerData[0].CastingPeriod,
                CastingDescription = _mockTechPowerData[0].CastingDescription,
                Range = Generation.RandomEnum<PowerRange>(),
                RangeDescription = Generation.Nonsense(50),
                Duration = _mockTechPowerData[0].Duration,
                Concentration = Faker.Boolean.Random(),
                HitOption = _mockTechPowerData[0].HitOption,
                HitDescription = Generation.Nonsense(50),
                Overcharge = _mockTechPowerData[0].Overcharge
            };
            var originalCount = _mockTechPowerData.Count;
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = await techPowerController.UpdateTechPowerById(Guid.NewGuid(), editTechPower);

            result.Should().BeOfType<CreatedResult>();
            _mockTechPowerData.Should().HaveCount(originalCount + 1);
            var addedTechPower = _mockTechPowerData.Last();
            Validation.CompareData(editTechPower, addedTechPower);
        }


        [Fact]
        public void Get_AllTechPowers_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<TechPower>(MockHttpContext.CurrentUser.UserName, null))
                .Throws(new Exception());
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = techPowerController.GetAllTechPowers();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_TechPowerById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<TechPower>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = techPowerController.GetTechPowerById(_mockTechPowerData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = await techPowerController.CreateNewTechPower(new TechPowerRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateTechPowerById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = await techPowerController.UpdateTechPowerById(_mockTechPowerData[0].Id, new TechPowerRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllTechPowersWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<TechPower>(It.IsAny<string>(), null))
                .Returns(_mockTechPowerData);
            mockRepo.Setup(x => x.GetAllEntities<TechPower>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(new List<TechPower>());
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);

            var result = techPowerController.GetAllTechPowers();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<TechPowerResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_TechPowerById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<TechPower>(_mockTechPowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockTechPowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockTechPowerData.Remove((TechPower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var techPower = _mockTechPowerData[0];
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);
            var result = techPowerController.DeleteTechPowerById(techPower.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockTechPowerData.Should().NotContain(techPower);
        }

        [Fact]
        public void Delete_TechPowerByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<TechPower>(_mockTechPowerData[0].Id, It.IsAny<string>()))
                .Returns(_mockTechPowerData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockTechPowerData.Remove((TechPower)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var techPowerController = CreateMockTechPowerController(mockRepo.Object);
            var result = techPowerController.DeleteTechPowerById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
