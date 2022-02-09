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
    public class EncounterTests
    {
        private readonly MockLogger<EncountersController> _mockLogger;
        private readonly ControllerUnitTestData<Encounter> _testData;
        private readonly Mapper _mapper;

        public EncounterTests()
        {
            _testData = new ControllerUnitTestData<Encounter>(Generation.RandomList(Generation.Encounter, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<EncountersController>();
        }

        private EncountersController CreateMockEncounterController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();

            var encounterController = new EncountersController(
                repo,
                _mockLogger,
                _mapper,
                MockUserManagerFactory.Create());

            encounterController.ControllerContext.HttpContext = httpContextMock;
            return encounterController;
        }

        [Fact]
        public void Get_AllEncounters_Success()
        {
            var repo = MockRepositories.GetAllEntities(_testData);
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.GetAllEncounters();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_EncounterById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.GetEncounterById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newEncounter = Generation.EncounterRequest();
            _testData.Expected = _mapper.Map<Encounter>(newEncounter);
            var encounterController = CreateMockEncounterController(repo);

            var result = await encounterController.CreateNewEncounter(newEncounter);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateEncounterById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editEncounter = Generation.EncounterRequest();
            _testData.Expected = _mapper.Map<Encounter>(editEncounter);
            var encounterController = CreateMockEncounterController(repo);

            var result = await encounterController.UpdateEncounterById(_testData.Entity.Id, editEncounter);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewEncounterById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editEncounter = Generation.EncounterRequest();
            _testData.Expected = _mapper.Map<Encounter>(editEncounter);
            var encounterController = CreateMockEncounterController(repo);

            var result = await encounterController.UpdateEncounterById(Guid.NewGuid(), editEncounter);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllEncounters_Failure()
        {
            var repo = MockRepositories.Failure<Encounter>();
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.GetAllEncounters();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_EncounterById_Failure()
        {
            var repo = MockRepositories.Failure<Encounter>();
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.GetEncounterById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewEncounter_Failure()
        {
            var repo = MockRepositories.Failure<Encounter>();
            var encounterController = CreateMockEncounterController(repo);

            var result = await encounterController.CreateNewEncounter(new EncounterRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateEncounterById_Failure()
        {
            var repo = MockRepositories.Failure<Encounter>();
            var encounterController = CreateMockEncounterController(repo);

            var result = await encounterController.UpdateEncounterById(_testData.Entity.Id, new EncounterRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllEncountersWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            _testData.ExpectedList = new List<Encounter>();
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.GetAllEncounters();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_EncounterById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.DeleteEncounterById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_EncounterByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var encounterController = CreateMockEncounterController(repo);

            var result = encounterController.DeleteEncounterById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
