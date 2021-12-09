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
    public class CreatureTests
    {
        readonly List<Creature> _mockCreatureData = new()
        {
            Generation.Creature()
        };
        readonly MockLogger<CreaturesController> _mockLogger;

        public CreatureTests()
        {
            _mockLogger = new MockLogger<CreaturesController>();
        }

        CreaturesController CreateMockCreatureController(IRepository repo)
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
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Creature>(MockHttpContext.CurrentUser.UserName))
                .Returns(_mockCreatureData);
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = creatureController.GetAllCreatures();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var val = okResult.Subject.Value.Should().BeOfType<List<CreatureResponse>>();
            val.Subject.Should().HaveCount(1);
            Validation.CompareData(_mockCreatureData[0], val.Subject[0]);
        }

        [Fact]
        public void Get_CreatureById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Creature>(_mockCreatureData[0].Id, It.IsAny<string>()))
                .Returns(_mockCreatureData[0]);
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = creatureController.GetCreatureById(_mockCreatureData[0].Id);

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Value.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<CreatureResponse>();
            Validation.CompareData(_mockCreatureData[0], response.Subject);
        }

        [Fact]
        public async Task Post_CreateNewCreature_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCreatureData.Add((Creature)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var newCreature = Generation.CreatureRequest();
            var originalCount = _mockCreatureData.Count;
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = await creatureController.CreateNewCreature(newCreature);

            result.Should().BeOfType<CreatedResult>();
            _mockCreatureData.Should().HaveCount(originalCount + 1);
            var addedCreature = _mockCreatureData.Last();
            Validation.CompareData(newCreature, addedCreature);
        }

        [Fact]
        public async Task Put_UpdateCreatureById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Creature>(_mockCreatureData[0].Id, It.IsAny<string>()))
                .Returns(_mockCreatureData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editCreature = new CreatureRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = _mockCreatureData[0].HitPoints,
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = _mockCreatureData[0].Speed,
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = _mockCreatureData[0].Dexterity,
                DexterityBonus = _mockCreatureData[0].DexterityBonus,
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = _mockCreatureData[0].Intelligence,
                IntelligenceBonus = _mockCreatureData[0].IntelligenceBonus,
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = _mockCreatureData[0].Charisma,
                CharismaBonus = _mockCreatureData[0].CharismaBonus,
                ChallengeRating = Faker.RandomNumber.Next(),
                Actions = _mockCreatureData[0].Actions,
                Vulnerabilities = Generation.DamageTypes(),
                Immunities = _mockCreatureData[0].Immunities,
                Resistances = Generation.DamageTypes(),
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = _mockCreatureData[0].TechPowerIds
            };
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = await creatureController.UpdateCreatureById(_mockCreatureData[0].Id, editCreature);

            result.Should().BeOfType<NoContentResult>();
            Validation.CompareData(editCreature, _mockCreatureData[0]);
        }

        [Fact]
        public async Task Put_NewCreatureById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCreatureData.Add((Creature)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var editCreature = new CreatureRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = _mockCreatureData[0].HitPoints,
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = _mockCreatureData[0].Speed,
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = _mockCreatureData[0].Dexterity,
                DexterityBonus = _mockCreatureData[0].DexterityBonus,
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = _mockCreatureData[0].Intelligence,
                IntelligenceBonus = _mockCreatureData[0].IntelligenceBonus,
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = _mockCreatureData[0].Charisma,
                CharismaBonus = _mockCreatureData[0].CharismaBonus,
                ChallengeRating = Faker.RandomNumber.Next(),
                Actions = _mockCreatureData[0].Actions,
                Vulnerabilities = Generation.DamageTypes(),
                Immunities = _mockCreatureData[0].Immunities,
                Resistances = Generation.DamageTypes(),
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = _mockCreatureData[0].TechPowerIds
            };
            var originalCount = _mockCreatureData.Count;
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = await creatureController.UpdateCreatureById(Guid.NewGuid(), editCreature);

            result.Should().BeOfType<CreatedResult>();
            _mockCreatureData.Should().HaveCount(originalCount + 1);
            var addedCreature = _mockCreatureData.Last();
            Validation.CompareData(editCreature, addedCreature);
        }


        [Fact]
        public void Get_AllCreatures_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Creature>(MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = creatureController.GetAllCreatures();

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_CreatureById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Creature>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = creatureController.GetCreatureById(_mockCreatureData[0].Id);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Post_CreateNewCreature_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = await creatureController.CreateNewCreature(new CreatureRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_UpdateCreatureById_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
               .Callback((object obj) => throw new Exception());
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = await creatureController.UpdateCreatureById(_mockCreatureData[0].Id, new CreatureRequest());

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().NotBeEmpty();
        }

        [Fact]
        public void Get_AllCreaturesWithWrongUser_ReturnsEmptyList()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<Creature>(It.IsAny<string>()))
                .Returns(_mockCreatureData);
            mockRepo.Setup(x => x.GetAllEntities<Creature>(MockHttpContext.CurrentUser.UserName))
                .Returns(new List<Creature>());
            var creatureController = CreateMockCreatureController(mockRepo.Object);

            var result = creatureController.GetAllCreatures();

            var okResult = result.Should().BeOfType<OkObjectResult>();
            okResult.Subject.Should().NotBeNull();
            var response = okResult.Subject.Value.Should().BeOfType<List<CreatureResponse>>();
            response.Subject.Should().BeEmpty();
            _mockLogger.Logs.Where(x => x.LogLevel == LogLevel.Error).Should().BeEmpty();
        }

        [Fact]
        public void Delete_CreatureById_Success()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Creature>(_mockCreatureData[0].Id, It.IsAny<string>()))
                .Returns(_mockCreatureData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCreatureData.Remove((Creature)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var creature = _mockCreatureData[0];
            var creatureController = CreateMockCreatureController(mockRepo.Object);
            var result = creatureController.DeleteCreatureById(creature.Id);

            result.Should().BeOfType<NoContentResult>();
            _mockCreatureData.Should().NotContain(creature);
        }

        [Fact]
        public void Delete_CreatureByInvalidId_Failure()
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<Creature>(_mockCreatureData[0].Id, It.IsAny<string>()))
                .Returns(_mockCreatureData[0]);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => _mockCreatureData.Remove((Creature)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            var creatureController = CreateMockCreatureController(mockRepo.Object);
            var result = creatureController.DeleteCreatureById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
