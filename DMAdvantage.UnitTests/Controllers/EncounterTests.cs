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

namespace DMAdvantage.UnitTests.Controllers
{
    public class EncounterTests
    {
        readonly List<Encounter> _mockEncounterData = new()
        {
            new Encounter
            {
                CharacterIds = new() { Guid.NewGuid(), Guid.NewGuid() },
                CreatureIds = new() { Guid.NewGuid(), Guid.NewGuid() },
                User = MockHttpContext.CurrentUser
            }
        };
        readonly MockLogger<EncountersController> _mockLogger;

        public EncounterTests()
        {
            _mockLogger = new MockLogger<EncountersController>();
        }

        EncountersController CreateMockEncounterController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var encounterController = new EncountersController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            encounterController.ControllerContext.HttpContext = httpContextMock;
            return encounterController;
        }

        [Fact]
        public void Get_AllEncounters_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Encounter>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Returns(_mockEncounterData);
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = encounterController.GetAllEncounters();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<EncounterResponse>>();
            response.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockEncounterData[0], response.Subject[0]);
        }

        [Fact]
        public void Get_EncounterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Encounter>(_mockEncounterData[0].Id, It.IsAny<string>()))
                .Returns(_mockEncounterData[0]);
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = encounterController.GetEncounterById(_mockEncounterData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<EncounterResponse>();
            Validation.CompareData(_mockEncounterData[0], response);
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockEncounterData.Add((Encounter)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newEncounter = new EncounterRequest
            {
                CharacterIds = new() { Guid.NewGuid(), Guid.NewGuid() },
                CreatureIds = new() { Guid.NewGuid(), Guid.NewGuid() }
            };
            var originalCount = _mockEncounterData.Count;
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = await encounterController.CreateNewEncounter(newEncounter);

            result.Should().BeOfType<CreatedResult>();
            _mockEncounterData.Should().HaveCount(originalCount + 1);
            var addedEncounter = _mockEncounterData.Last();
            Validation.CompareData(newEncounter, addedEncounter);
        }

        [Fact]
        public async Task Put_UpdateEncounterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Encounter>(_mockEncounterData[0].Id, It.IsAny<string>()))
                .Returns(_mockEncounterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editEncounter = new EncounterRequest
            {
                CharacterIds = _mockEncounterData[0].CharacterIds,
                CreatureIds = new() { Guid.NewGuid(), Guid.NewGuid() }
            };
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = await encounterController.UpdateEncounterById(_mockEncounterData[0].Id, editEncounter);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editEncounter, _mockEncounterData[0]);
        }

        [Fact]
        public async Task Put_NewEncounterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockEncounterData.Add((Encounter)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editEncounter = new EncounterRequest
            {
                CharacterIds = _mockEncounterData[0].CharacterIds,
                CreatureIds = new() { Guid.NewGuid(), Guid.NewGuid() }
            };
            var originalCount = _mockEncounterData.Count;
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = await encounterController.UpdateEncounterById(Guid.NewGuid(), editEncounter);

            result.Should().BeOfType<CreatedResult>();
            _mockEncounterData.Should().HaveCount(originalCount + 1);
            var addedEncounter = _mockEncounterData.Last();
            Validation.CompareData(editEncounter, addedEncounter);
        }


        [Fact]
        public void Get_AllEncounters_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Encounter>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Throws(new Exception());
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = encounterController.GetAllEncounters();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_EncounterById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Encounter>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = encounterController.GetEncounterById(_mockEncounterData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = await encounterController.CreateNewEncounter(new EncounterRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateEncounterById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = await encounterController.UpdateEncounterById(_mockEncounterData[0].Id, new EncounterRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllEncountersWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Encounter>(It.IsAny<string>(), It.IsAny<PagingParameters?>()))
                .Returns(_mockEncounterData);
            mockRepo.Setup(x => x.GetAllEntities<Encounter>(MockHttpContext.CurrentUser.UserName, It.IsAny<PagingParameters?>()))
                .Returns(new List<Encounter>());
            var encounterController = CreateMockEncounterController(mockRepo.Object);

            var result = encounterController.GetAllEncounters();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<EncounterResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_EncounterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Encounter>(_mockEncounterData[0].Id, It.IsAny<string>()))
                .Returns(_mockEncounterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockEncounterData.Remove((Encounter)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var encounter = _mockEncounterData[0];
            var encounterController = CreateMockEncounterController(mockRepo.Object);
            var result = encounterController.DeleteEncounterById(encounter.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockEncounterData.Should().NotContain(encounter);
        }

        [Fact]
        public void Delete_EncounterByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Encounter>(_mockEncounterData[0].Id, It.IsAny<string>()))
                .Returns(_mockEncounterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockEncounterData.Remove((Encounter)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var encounterController = CreateMockEncounterController(mockRepo.Object);
            var result = encounterController.DeleteEncounterById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
