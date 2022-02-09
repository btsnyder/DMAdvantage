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
    public class CreatureTests
    {
        private readonly MockLogger<CreaturesController> _mockLogger;
        private readonly ControllerUnitTestData<Creature> _testData;
        private readonly Mapper _mapper;

        public CreatureTests()
        {
            _testData = new ControllerUnitTestData<Creature>(Generation.RandomList(Generation.Creature, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<CreaturesController>();
        }

        private CreaturesController CreateMockCreatureController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var creatureController = new CreaturesController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            creatureController.ControllerContext.HttpContext = httpContextMock;
            return creatureController;
        }

        [Fact]
        public void Get_AllCreatures_Success()
        {
            var repo = MockRepositories.GetAllEntities(_testData);
            var creatureController = CreateMockCreatureController(repo);

            var result = creatureController.GetAllCreatures();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_CreatureById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var creatureController = CreateMockCreatureController(repo);

            var result = creatureController.GetCreatureById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewCreature_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newCreature = Generation.CreatureRequest();
            _testData.Expected = _mapper.Map<Creature>(newCreature);
            var creatureController = CreateMockCreatureController(repo);

            var result = await creatureController.CreateNewCreature(newCreature);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateCreatureById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editCreature = Generation.CreatureRequest();
            _testData.Expected = _mapper.Map<Creature>(editCreature);
            var creatureController = CreateMockCreatureController(repo);

            var result = await creatureController.UpdateCreatureById(_testData.Entity.Id, editCreature);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewCreatureById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editCreature = Generation.CreatureRequest();
            _testData.Expected = _mapper.Map<Creature>(editCreature);
            var creatureController = CreateMockCreatureController(repo);

            var result = await creatureController.UpdateCreatureById(Guid.NewGuid(), editCreature);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllCreatures_Failure()
        {
            var repo = MockRepositories.Failure<Creature>();
            var creatureController = CreateMockCreatureController(repo);

            var result = creatureController.GetAllCreatures();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_CreatureById_Failure()
        {
            var repo = MockRepositories.Failure<Creature>();
            var creatureController = CreateMockCreatureController(repo);

            var result = creatureController.GetCreatureById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewCreature_Failure()
        {
            var repo = MockRepositories.Failure<Creature>();
            var creatureController = CreateMockCreatureController(repo);

            var result = await creatureController.CreateNewCreature(new CreatureRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateCreatureById_Failure()
        {
            var repo = MockRepositories.Failure<Creature>();
            var creatureController = CreateMockCreatureController(repo);

            var result = await creatureController.UpdateCreatureById(_testData.Entity.Id, new CreatureRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllCreaturesWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            var creatureController = CreateMockCreatureController(repo);
            _testData.ExpectedList = new List<Creature>();

            var result = creatureController.GetAllCreatures();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_CreatureById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var creatureController = CreateMockCreatureController(repo);
            var result = creatureController.DeleteCreatureById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_CreatureByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var creatureController = CreateMockCreatureController(repo);
            var result = creatureController.DeleteCreatureById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
