using TestEngineering;
using TestEngineering.Mocks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Xunit;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Server.Controllers;
using DMAdvantage.Data;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using TestEngineering.Enums;

namespace DMAdvantage.UnitTests.Controllers
{
    public class ForcePowerTests
    {
        private readonly MockLogger<ForcePowersController> _mockLogger;
        private readonly ControllerUnitTestData<ForcePower> _testData;
        private readonly Mapper _mapper;

        public ForcePowerTests()
        {
            _testData = new ControllerUnitTestData<ForcePower>(Generation.RandomList(Generation.ForcePower, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<ForcePowersController>();
        }

        private ForcePowersController CreateMockForcePowerController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();

            var forcePowerController = new ForcePowersController(
                repo,
                _mockLogger,
                _mapper,
                MockUserManagerFactory.Create());

            forcePowerController.ControllerContext.HttpContext = httpContextMock;
            return forcePowerController;
        }

        [Fact]
        public void Get_AllForcePowers_Success()
        {
            var repo = MockRepositories.GetAllEntities(_testData);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = forcePowerController.GetAllForcePowers();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_ForcePowerById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = forcePowerController.GetForcePowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newForcePower = Generation.ForcePowerRequest();
            _testData.Expected = _mapper.Map<ForcePower>(newForcePower);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = await forcePowerController.CreateNewForcePower(newForcePower);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateForcePowerById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editForcePower = Generation.ForcePowerRequest();
            _testData.Expected = _mapper.Map<ForcePower>(editForcePower);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = await forcePowerController.UpdateForcePowerById(_testData.Entity.Id, editForcePower);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewForcePowerById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editForcePower = Generation.ForcePowerRequest();
            _testData.Expected = _mapper.Map<ForcePower>(editForcePower);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = await forcePowerController.UpdateForcePowerById(Guid.NewGuid(), editForcePower);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllForcePowers_Failure()
        {
            var repo = MockRepositories.Failure<ForcePower>();
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = forcePowerController.GetAllForcePowers();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_ForcePowerById_Failure()
        {
            var repo = MockRepositories.Failure<ForcePower>();
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = forcePowerController.GetForcePowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewForcePower_Failure()
        {
            var repo = MockRepositories.Failure<ForcePower>();
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = await forcePowerController.CreateNewForcePower(new ForcePowerRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateForcePowerById_Failure()
        {
            var repo = MockRepositories.Failure<ForcePower>();
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = await forcePowerController.UpdateForcePowerById(_testData.Entity.Id, new ForcePowerRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllForcePowersWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            var forcePowerController = CreateMockForcePowerController(repo);
            _testData.ExpectedList = new List<ForcePower>();

            var result = forcePowerController.GetAllForcePowers();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Delete_ForcePowerById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var forcePowerController = CreateMockForcePowerController(repo);
            
            var result = forcePowerController.DeleteForcePowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_ForcePowerByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var forcePowerController = CreateMockForcePowerController(repo);

            var result = forcePowerController.DeleteForcePowerById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
