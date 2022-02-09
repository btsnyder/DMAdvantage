using AutoMapper;
using TestEngineering;
using TestEngineering.Mocks;
using Microsoft.Extensions.Logging;
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
using TestEngineering.Enums;

namespace DMAdvantage.UnitTests.Controllers
{
    public class TechPowerTests
    {
        private readonly MockLogger<TechPowersController> _mockLogger;
        private readonly ControllerUnitTestData<TechPower> _testData;
        protected readonly Mapper _mapper;

        public TechPowerTests()
        {
            _testData = new ControllerUnitTestData<TechPower>(Generation.RandomList(Generation.TechPower, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<TechPowersController>();
        }

        private TechPowersController CreateMockTechPowerController(IRepository repo)
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
            var repo = MockRepositories.GetAllEntities(_testData);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.GetAllTechPowers();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_TechPowerById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.GetTechPowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newTechPower = Generation.TechPowerRequest();
            _testData.Expected = _mapper.Map<TechPower>(newTechPower);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = await techPowerController.CreateNewTechPower(newTechPower);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateTechPowerById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editTechPower = Generation.TechPowerRequest();
            _testData.Expected = _mapper.Map<TechPower>(editTechPower);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = await techPowerController.UpdateTechPowerById(_testData.Entity.Id, editTechPower);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewTechPowerById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editTechPower = Generation.TechPowerRequest();
            _testData.Expected = _mapper.Map<TechPower>(editTechPower);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = await techPowerController.UpdateTechPowerById(Guid.NewGuid(), editTechPower);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllTechPowers_Failure()
        {
            var repo = MockRepositories.Failure<TechPower>();
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.GetAllTechPowers();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_TechPowerById_Failure()
        {
            var repo = MockRepositories.Failure<TechPower>();
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.GetTechPowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewTechPower_Failure()
        {
            var repo = MockRepositories.Failure<TechPower>();
            var techPowerController = CreateMockTechPowerController(repo);

            var result = await techPowerController.CreateNewTechPower(new TechPowerRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateTechPowerById_Failure()
        {
            var repo = MockRepositories.Failure<TechPower>();
            var techPowerController = CreateMockTechPowerController(repo);

            var result = await techPowerController.UpdateTechPowerById(_testData.Entity.Id, new TechPowerRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllTechPowersWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            _testData.ExpectedList = new List<TechPower>();
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.GetAllTechPowers();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_TechPowerById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var techPowerController = CreateMockTechPowerController(repo);
            
            var result = techPowerController.DeleteTechPowerById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_TechPowerByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var techPowerController = CreateMockTechPowerController(repo);

            var result = techPowerController.DeleteTechPowerById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
