using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.UnitTests.Data
{
    public class BaseEntityTests
    {
        protected readonly Repository _mockRepo;

        public BaseEntityTests()
        {
            var contextOptions = new DbContextOptionsBuilder<Context>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var mockContext = new Context(contextOptions, null);
            _mockRepo = new Repository(mockContext);
        }

        protected void GetAllEntities_Success<T>(T entity) where T : BaseEntity
        {
            _mockRepo.AddEntity(entity);
            _mockRepo.SaveAll();
            var repoEntities = _mockRepo.GetAllEntities<T>(entity.User?.UserName ?? string.Empty).ToList();

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

            encounterFromRepo.Should().NotBeNull();
            encounterFromRepo!.Id.Should().Be(entity.Id);
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

        protected void GetEntitiesWithPaging_Success<T>(List<T> entities) where T : BaseEntity
        {
            foreach (var entity in entities)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();
            var paging = new PagingParameters
            {
                PageSize = 5,
                PageNumber = 2
            };
            var pagedEntities = _mockRepo.GetAllEntities<T>(entities[0].User?.UserName ?? string.Empty, paging);

            pagedEntities.Should().NotBeNull();
            pagedEntities.First().Id.Should().Be(entities[5].Id);
            pagedEntities.Should().HaveCount(paging.PageSize);
            pagedEntities.TotalCount.Should().Be(entities.Count);
        }

        protected void GetEntitiesWithSearching_Success<T>(List<T> entities, ISearchParameters<T> search, Func<T, bool> whereClause) where T : BaseEntity
        {
            if (_mockRepo.GetEntityByIdWithoutUser<T>(entities.First().Id) == null)
            {
                foreach (var entity in entities)
                {
                    _mockRepo.AddEntity(entity);
                }
                _mockRepo.SaveAll();
            }
            

            var paging = new PagingParameters
            {
                PageSize = 100,
                PageNumber = 1
            };

            var searchedEntities = _mockRepo.GetAllEntities<T>(entities[0].User!.UserName, paging, search);
            searchedEntities.Should().BeEquivalentTo(entities.Where(whereClause));
        }
    }
}
