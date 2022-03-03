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
    public class AbilityTests
    {
        private readonly MockLogger<AbilitiesController> _mockLogger;
        private readonly ControllerUnitTestData<Ability> _testData;
        private readonly Mapper _mapper;

        public AbilityTests()
        {
            _testData = new ControllerUnitTestData<Ability>(Generation.RandomList(Generation.Ability, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<AbilitiesController>();
        }

        private AbilitiesController CreateMockAbilitiesController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var abilitiesController = new AbilitiesController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            abilitiesController.ControllerContext.HttpContext = httpContextMock;
            return abilitiesController;
        }

        [Fact]
        public void Get_AllAbilities_Success()
        {
            var repo = MockRepositories.GetAllEntities(_testData);
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = abilitiesController.GetAllAbilities();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_AbilityById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = abilitiesController.GetAbilityById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewAbility_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newAbility = Generation.AbilityRequest();
            _testData.Expected = _mapper.Map<Ability>(newAbility);
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = await abilitiesController.CreateNewAbility(newAbility);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateAbilityById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editAbility = Generation.AbilityRequest();
            _testData.Expected = _mapper.Map<Ability>(editAbility);
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = await abilitiesController.UpdateAbilityById(_testData.Entity.Id, editAbility);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewAbilityById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editAbility = Generation.AbilityRequest();
            _testData.Expected = _mapper.Map<Ability>(editAbility);
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = await abilitiesController.UpdateAbilityById(Guid.NewGuid(), editAbility);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllAbilities_Failure()
        {
            var repo = MockRepositories.Failure<Ability>();
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = abilitiesController.GetAllAbilities();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AbilityById_Failure()
        {
            var repo = MockRepositories.Failure<Ability>();
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = abilitiesController.GetAbilityById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewAbility_Failure()
        {
            var repo = MockRepositories.Failure<Ability>();
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = await abilitiesController.CreateNewAbility(new AbilityRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateAbilityById_Failure()
        {
            var repo = MockRepositories.Failure<Ability>();
            var abilitiesController = CreateMockAbilitiesController(repo);

            var result = await abilitiesController.UpdateAbilityById(_testData.Entity.Id, new AbilityRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllAbilitiesWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            var abilitiesController = CreateMockAbilitiesController(repo);
            _testData.ExpectedList = new List<Ability>();

            var result = abilitiesController.GetAllAbilities();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_AbilityById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var abilitiesController = CreateMockAbilitiesController(repo);
            var result = abilitiesController.DeleteAbilityById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_AbilityByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var abilitiesController = CreateMockAbilitiesController(repo);
            var result = abilitiesController.DeleteAbilityById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
