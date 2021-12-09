using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TestEngineering.Mocks;

namespace DMAdvantage.UnitTests.Data
{
    public class BaseEntityTests
    {
        private readonly MockLogger<Repository> _mockLogger;
        private readonly Context _mockContext;
        private readonly Repository _mockRepo;

        public BaseEntityTests()
        {
            _mockLogger = new MockLogger<Repository>();
            var contextOptions = new DbContextOptionsBuilder<Context>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _mockContext = new Context(contextOptions, null);
            _mockRepo = new Repository(_mockContext, _mockLogger);
        }

        protected void GetAllEntities_Success<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var repoEntities = _mockRepo.GetAllEntities<T>(entity.User?.UserName ?? string.Empty);

            repoEntities.Should().HaveCount(1);
            repoEntities.First().Id.Should().Be(entity.Id);
        }

        protected void GetAllEntitiesFromDifferentUser_EmptyList<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var repoCreatures = _mockRepo.GetAllEntities<T>("newuser");

            repoCreatures.Should().BeEmpty();
        }

        protected void GetEntityById_Success<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var encounterFromRepo = _mockRepo.GetEntityById<T>(entity.Id, entity.User?.UserName ?? string.Empty);

            encounterFromRepo.Id.Should().Be(entity.Id);
        }

        protected void GetEntityByIdFromDifferentUser_Null<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var entityFromRepo = _mockRepo.GetEntityById<T>(entity.Id, "newuser");

            entityFromRepo.Should().BeNull();
        }

        protected void GetEntityByBadId_Null<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var entityFromRepo = _mockRepo.GetEntityById<T>(Guid.NewGuid(), entity.User?.UserName ?? string.Empty);

            entityFromRepo.Should().BeNull();
        }
    }
}
