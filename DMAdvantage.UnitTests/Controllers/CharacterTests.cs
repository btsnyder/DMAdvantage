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
    public class CharacterTests
    {
        private readonly MockLogger<CharactersController> _mockLogger;
        private readonly ControllerUnitTestData<Character> _testData;
        private readonly Mapper _mapper;

        public CharacterTests()
        {
            _testData = new ControllerUnitTestData<Character>(Generation.RandomList(Generation.Character, generateMax: true));

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
            _mockLogger = new MockLogger<CharactersController>();
        }

        private CharactersController CreateMockCharacterController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var characterController = new CharactersController(
                repo,
                _mockLogger,
                _mapper,
                MockUserManagerFactory.Create());

            characterController.ControllerContext.HttpContext = httpContextMock;
            return characterController;
        }

        [Fact]
        public void Get_AllCharacters_Success()
        {
            var repo = MockRepositories.GetAllEntities(_testData);
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.GetAllCharacters();
            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public void Get_CharacterById_Success()
        {
            var repo = MockRepositories.GetEntityById(_testData);
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.GetCharacterById(_testData.Entity.Id);
            Validation.ValidateResponse(TestAction.Get, result, _testData);
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var newCharacter = Generation.CharacterRequest();
            _testData.Expected = _mapper.Map<Character>(newCharacter);
            var characterController = CreateMockCharacterController(repo);

            var result = await characterController.CreateNewCharacter(newCharacter);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateCharacterById_Success()
        {
            var repo = MockRepositories.UpdateEntity(_testData);
            var editCharacter = Generation.CharacterRequest();
            _testData.Expected = _mapper.Map<Character>(editCharacter);
            var characterController = CreateMockCharacterController(repo);

            var result = await characterController.UpdateCharacterById(_testData.Entity.Id, editCharacter);

            Validation.ValidateResponse(TestAction.Update, result, _testData);
        }

        [Fact]
        public async Task Put_NewCharacterById_Success()
        {
            var repo = MockRepositories.CreateNewEntity(_testData);
            var editCharacter = Generation.CharacterRequest();
            _testData.Expected = _mapper.Map<Character>(editCharacter);
            var characterController = CreateMockCharacterController(repo);

            var result = await characterController.UpdateCharacterById(Guid.NewGuid(), editCharacter);

            Validation.ValidateResponse(TestAction.Create, result, _testData);
        }


        [Fact]
        public void Get_AllCharacters_Failure()
        {
            var repo = MockRepositories.Failure<Character>();
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.GetAllCharacters();

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_CharacterById_Failure()
        {
            var repo = MockRepositories.Failure<Character>();
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.GetCharacterById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Failure()
        {
            var repo = MockRepositories.Failure<Character>();
            var characterController = CreateMockCharacterController(repo);

            var result = await characterController.CreateNewCharacter(new CharacterRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
        }

        [Fact]
        public async Task Put_UpdateCharacterById_Failure()
        {
            var repo = MockRepositories.Failure<Character>();
            var characterController = CreateMockCharacterController(repo);

            var result = await characterController.UpdateCharacterById(_testData.Entity.Id, new CharacterRequest());

            Validation.ValidateResponse(TestAction.Error, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllCharactersWithWrongUser_ReturnsEmptyList()
        {
            var repo = MockRepositories.GetEmptyEntities(_testData);
            _testData.ExpectedList = new List<Character>();
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.GetAllCharacters();

            Validation.ValidateResponse(TestAction.Get, result, _testData);
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_CharacterById_Success()
        {
            var repo = MockRepositories.DeleteEntity(_testData);
            var characterController = CreateMockCharacterController(repo);

            var result = characterController.DeleteCharacterById(_testData.Entity.Id);

            Validation.ValidateResponse(TestAction.Delete, result, _testData);
        }

        [Fact]
        public void Delete_CharacterByInvalidId_Failure()
        {
            var repo = MockRepositories.DeleteEntity(_testData); 
            var characterController = CreateMockCharacterController(repo);
            var result = characterController.DeleteCharacterById(Guid.NewGuid());

            Validation.ValidateResponse(TestAction.Missing, result, _testData);
        }
    }
}
