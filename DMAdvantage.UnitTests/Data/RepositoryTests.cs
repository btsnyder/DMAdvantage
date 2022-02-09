using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class RepositoryTests
    {
        private readonly Context _mockContext;
        private readonly Repository _mockRepo;

        public RepositoryTests()
        {
            var contextOptions = new DbContextOptionsBuilder<Context>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _mockContext = new Context(contextOptions, null);
            _mockRepo = new Repository(_mockContext);
        }

        [Fact]
        public void AddEntities_Success()
        {
            var character = Generation.Character();
            var creature = Generation.Creature();
            var data = new List<InitativeData>
            {
                new() { BeingId = character.Id },
                new() { BeingId = creature.Id }
            };
            var encounter = new Encounter { DataCache = JsonSerializer.Serialize(data) };

            _mockRepo.AddEntity(character);
            _mockRepo.AddEntity(creature);
            _mockRepo.AddEntity(encounter);

            _mockRepo.SaveAll().Should().BeTrue();
            _mockContext.Characters.Should().HaveCount(1);
            _mockContext.Characters.First().Id.Should().Be(character.Id);
            _mockContext.Creatures.Should().HaveCount(1);
            _mockContext.Creatures.First().Id.Should().Be(creature.Id);
            _mockContext.Encounters.Should().HaveCount(1);
            _mockContext.Encounters.First().Id.Should().Be(encounter.Id);
        }

        [Fact]
        public void RemoveEntity_Success()
        {
            var character = Generation.Character();
            _mockRepo.AddEntity(character);
            _mockRepo.SaveAll().Should().BeTrue();
            _mockContext.Characters.Should().HaveCount(1);
            var characterFromRepo = _mockContext.Characters.First();
            _mockRepo.RemoveEntity(characterFromRepo);
            _mockRepo.SaveAll().Should().BeTrue();
            _mockContext.Characters.Should().BeEmpty();
        }

        [Fact]
        public void AddExistingEntity_Failure()
        {
            var character = Generation.Character();

            _mockRepo.AddEntity(character);
            _mockRepo.SaveAll();
            _mockRepo.AddEntity(_mockContext.Characters.First());

            Action act = () => _mockRepo.SaveAll();

            act.Should().Throw<ArgumentException>();
        }
    }
}
