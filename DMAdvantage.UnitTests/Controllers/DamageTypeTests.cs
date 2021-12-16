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
    public class DamageTypeTests
    {
        readonly List<DamageType> _mockDamageTypeData = new()
        {
            Generation.DamageType()
        };
        readonly MockLogger<DamageTypesController> _mockLogger;

        public DamageTypeTests()
        {
            _mockLogger = new MockLogger<DamageTypesController>();
        }

        DamageTypesController CreateMockDamageTypeController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var damageTypeController = new DamageTypesController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            damageTypeController.ControllerContext.HttpContext = httpContextMock;
            return damageTypeController;
        }

        [Fact]
        public void Get_AllDamageTypes_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<DamageType>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(_mockDamageTypeData);
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = damageTypeController.GetAllDamageTypes();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var val = okResult.Subject.Value.Should().BeOfType<List<DamageTypeResponse>>();
            val.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockDamageTypeData[0], val.Subject[0]);
        }

        [Fact]
        public void Get_DamageTypeById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<DamageType>(_mockDamageTypeData[0].Id, It.IsAny<string>()))
                .Returns(_mockDamageTypeData[0]);
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = damageTypeController.GetDamageTypeById(_mockDamageTypeData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<DamageTypeResponse>();
            Validation.CompareData(_mockDamageTypeData[0], response.Subject);
        }

        [Fact]
        public async Task Post_CreateNewDamageType_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockDamageTypeData.Add((DamageType)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newDamageType = Generation.DamageTypeRequest();
            var originalCount = _mockDamageTypeData.Count;
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = await damageTypeController.CreateNewDamageType(newDamageType);

            result.Should().BeOfType<CreatedResult>();
            _mockDamageTypeData.Should().HaveCount(originalCount + 1);
            var addedDamageType = _mockDamageTypeData.Last();
            Validation.CompareData(newDamageType, addedDamageType);
        }

        [Fact]
        public async Task Put_UpdateDamageTypeById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<DamageType>(_mockDamageTypeData[0].Id, It.IsAny<string>()))
                .Returns(_mockDamageTypeData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editDamageType = Generation.DamageTypeRequest();
            
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = await damageTypeController.UpdateDamageTypeById(_mockDamageTypeData[0].Id, editDamageType);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editDamageType, _mockDamageTypeData[0]);
        }

        [Fact]
        public async Task Put_NewDamageTypeById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockDamageTypeData.Add((DamageType)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editDamageType = Generation.DamageTypeRequest();
            
            var originalCount = _mockDamageTypeData.Count;
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = await damageTypeController.UpdateDamageTypeById(Guid.NewGuid(), editDamageType);

            result.Should().BeOfType<CreatedResult>();
            _mockDamageTypeData.Should().HaveCount(originalCount + 1);
            var addedDamageType = _mockDamageTypeData.Last();
            Validation.CompareData(editDamageType, addedDamageType);
        }


        [Fact]
        public void Get_AllDamageTypes_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<DamageType>(MockHttpContext.CurrentUser.UserName, null))
                .Throws(new Exception());
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = damageTypeController.GetAllDamageTypes();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_DamageTypeById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<DamageType>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = damageTypeController.GetDamageTypeById(_mockDamageTypeData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewDamageType_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = await damageTypeController.CreateNewDamageType(new DamageTypeRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateDamageTypeById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = await damageTypeController.UpdateDamageTypeById(_mockDamageTypeData[0].Id, new DamageTypeRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllDamageTypesWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<DamageType>(It.IsAny<string>(), null))
                .Returns(_mockDamageTypeData);
            mockRepo.Setup(x => x.GetAllEntities<DamageType>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(new List<DamageType>());
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);

            var result = damageTypeController.GetAllDamageTypes();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<DamageTypeResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_DamageTypeById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<DamageType>(_mockDamageTypeData[0].Id, It.IsAny<string>()))
                .Returns(_mockDamageTypeData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockDamageTypeData.Remove((DamageType)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var DamageType = _mockDamageTypeData[0];
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);
            var result = damageTypeController.DeleteDamageTypeById(DamageType.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockDamageTypeData.Should().NotContain(DamageType);
        }

        [Fact]
        public void Delete_DamageTypeByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<DamageType>(_mockDamageTypeData[0].Id, It.IsAny<string>()))
                .Returns(_mockDamageTypeData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockDamageTypeData.Remove((DamageType)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var damageTypeController = CreateMockDamageTypeController(mockRepo.Object);
            var result = damageTypeController.DeleteDamageTypeById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
