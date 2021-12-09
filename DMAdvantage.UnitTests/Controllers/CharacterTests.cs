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
    public class CharacterTests
    {
        readonly List<Character> _mockCharacterData = new()
        {
            Generation.Character()
        };
        readonly MockLogger<CharactersController> _mockLogger;

        public CharacterTests()
        {
            _mockLogger = new MockLogger<CharactersController>();
        }

        CharactersController CreateMockCharacterController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var characterController = new CharactersController(
                repo,
                _mockLogger,
                mapper,
                MockUserManagerFactory.Create());

            characterController.ControllerContext.HttpContext = httpContextMock;
            return characterController;
        }

        [Fact]
        public void Get_AllCharacters_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Character>(MockHttpContext.CurrentUser.UserName))
                .Returns(_mockCharacterData);
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = characterController.GetAllCharacters();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<CharacterResponse>>();
            response.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockCharacterData[0], response.Subject[0]);
        }

        [Fact]
        public void Get_CharacterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Character>(_mockCharacterData[0].Id, It.IsAny<string>()))
                .Returns(_mockCharacterData[0]);
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = characterController.GetCharacterById(_mockCharacterData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<CharacterResponse>();
            Validation.CompareData(_mockCharacterData[0], response.Subject);
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCharacterData.Add((Character)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newCharacter = Generation.CharacterRequest();
            var originalCount = _mockCharacterData.Count;
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = await characterController.CreateNewCharacter(newCharacter);

            result.Should().BeOfType<CreatedResult>();
            _mockCharacterData.Should().HaveCount(originalCount + 1);
            var addedCharacter = _mockCharacterData.Last();
            Validation.CompareData(newCharacter, addedCharacter);
        }

        [Fact]
        public async Task Put_UpdateCharacterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Character>(_mockCharacterData[0].Id, It.IsAny<string>()))
                .Returns(_mockCharacterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editCharacter = new CharacterRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = _mockCharacterData[0].HitPoints,
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = _mockCharacterData[0].Speed,
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = _mockCharacterData[0].Dexterity,
                DexterityBonus = _mockCharacterData[0].DexterityBonus,
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = _mockCharacterData[0].Intelligence,
                IntelligenceBonus = _mockCharacterData[0].IntelligenceBonus,
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = _mockCharacterData[0].Charisma,
                CharismaBonus = _mockCharacterData[0].CharismaBonus,
                PlayerName = Faker.Name.FullName(),
                Level = _mockCharacterData[0].Level,
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = _mockCharacterData[0].TechPowerIds
            };
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = await characterController.UpdateCharacterById(_mockCharacterData[0].Id, editCharacter);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editCharacter, _mockCharacterData[0]);
        }

        [Fact]
        public async Task Put_NewCharacterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCharacterData.Add((Character)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editCharacter = new CharacterRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = _mockCharacterData[0].HitPoints,
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = _mockCharacterData[0].Speed,
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = _mockCharacterData[0].Dexterity,
                DexterityBonus = _mockCharacterData[0].DexterityBonus,
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = _mockCharacterData[0].Intelligence,
                IntelligenceBonus = _mockCharacterData[0].IntelligenceBonus,
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = _mockCharacterData[0].Charisma,
                CharismaBonus = _mockCharacterData[0].CharismaBonus,
                PlayerName = Faker.Name.FullName(),
                Level = _mockCharacterData[0].Level,
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = _mockCharacterData[0].TechPowerIds
            };
            var originalCount = _mockCharacterData.Count;
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = await characterController.UpdateCharacterById(Guid.NewGuid(), editCharacter);

            result.Should().BeOfType<CreatedResult>();
            _mockCharacterData.Should().HaveCount(originalCount + 1);
            var addedCharacter = _mockCharacterData.Last();
            Validation.CompareData(editCharacter, addedCharacter);
        }


        [Fact]
        public void Get_AllCharacters_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Character>(MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = characterController.GetAllCharacters();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_CharacterById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Character>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = characterController.GetCharacterById(_mockCharacterData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewCharacter_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = await characterController.CreateNewCharacter(new CharacterRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateCharacterById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = await characterController.UpdateCharacterById(_mockCharacterData[0].Id, new CharacterRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllCharactersWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Character>(It.IsAny<string>()))
                .Returns(_mockCharacterData);
            mockRepo.Setup(x => x.GetAllEntities<Character>(MockHttpContext.CurrentUser.UserName))
                .Returns(new List<Character>());
            var characterController = CreateMockCharacterController(mockRepo.Object);

            var result = characterController.GetAllCharacters();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<CharacterResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_CharacterById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Character>(_mockCharacterData[0].Id, It.IsAny<string>()))
                .Returns(_mockCharacterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCharacterData.Remove((Character)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var character = _mockCharacterData[0];
            var characterController = CreateMockCharacterController(mockRepo.Object);
            var result = characterController.DeleteCharacterById(character.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockCharacterData.Should().NotContain(character);
        }

        [Fact]
        public void Delete_CharacterByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Character>(_mockCharacterData[0].Id, It.IsAny<string>()))
                .Returns(_mockCharacterData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCharacterData.Remove((Character)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var characterController = CreateMockCharacterController(mockRepo.Object);
            var result = characterController.DeleteCharacterById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
