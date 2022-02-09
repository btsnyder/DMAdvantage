using System.Linq;
using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Server.Controllers;
using DMAdvantage.Shared.Entities;
using TestEngineering;
using TestEngineering.Enums;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.UnitTests.Controllers
{
    public class ViewTests
    {
        private readonly ControllerUnitTestData<Encounter> _encounters = new(Generation.RandomList(Generation.Encounter, generateMax: true));
        private readonly ControllerUnitTestData<Character> _characters = new(Generation.RandomList(Generation.Character, generateMax: true));
        private readonly ControllerUnitTestData<Creature> _creatures = new(Generation.RandomList(Generation.Creature, generateMax: true));
        private readonly ControllerUnitTestData<ForcePower> _forcePowers = new(Generation.RandomList(Generation.ForcePower, generateMax: true));

        private readonly MockLogger<ViewController> _mockLogger;

        public ViewTests()
        {
            _mockLogger = new MockLogger<ViewController>();
        }

        private ViewController CreateMockViewController(IRepository repo)
        {
            var httpContextMock = new MockHttpContext();
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = new Mapper(config);

            var viewController = new ViewController(
                repo,
                _mockLogger,
                mapper);

            viewController.ControllerContext.HttpContext = httpContextMock;
            return viewController;
        }

        [Fact]
        public void Get_EncounterById_Success()
        {
            var repo = MockRepositories.GetEntityByIdWithoutUser(_encounters);
            var viewController = CreateMockViewController(repo);

            var result = viewController.GetEncounterById(_encounters.Entity.Id);

            Validation.ValidateResponse(TestAction.Get, result, _encounters);
        }

        [Fact]
        public void Get_CharacterByPlayerName_Success()
        {
            var repo = MockRepositories.GetCharacterByPlayerName(_characters);
            var viewController = CreateMockViewController(repo);

            var result = viewController.GetCharacterByPlayerName(_characters.Entity.PlayerName!);

            Validation.ValidateResponse(TestAction.Get, result, _characters);
        }

        [Fact]
        public void Get_CreaturesByIds_Success()
        {
            var ids = _creatures.RepositoryEntities.Select(x => x.Id).ToArray();
            var repo = MockRepositories.GetEntitiesByIdsWithoutUser(_creatures, ids);
            var viewController = CreateMockViewController(repo);

            var result = viewController.GetCreaturesByIds(_creatures.RepositoryEntities.Select(c => c.Id).ToArray());

            Validation.ValidateResponse(TestAction.Get, result, _creatures);
        }

        [Fact]
        public void Get_GetForcePowersByIds_Success()
        {
            var ids = _forcePowers.RepositoryEntities.Select(x => x.Id).ToArray();
            var repo = MockRepositories.GetEntitiesByIdsWithoutUser(_forcePowers, ids);
            var viewController = CreateMockViewController(repo);

            var result = viewController.GetForcePowersByIds(_forcePowers.RepositoryEntities.Select(f => f.Id).ToArray());

            Validation.ValidateResponse(TestAction.Get, result, _forcePowers);
        }
    }
}
