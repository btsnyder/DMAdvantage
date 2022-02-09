using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using FluentAssertions;
using System;
using System.Collections.Generic;
using TestEngineering;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class ForcePowerTests : BaseEntityTests
    {
        [Fact]
        public void GetAllForcePowers_Success()
        {
            GetAllEntities_Success(Generation.ForcePower());
        }

        [Fact]
        public void GetAllForcePowersFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.ForcePower());
        }

        [Fact]
        public void GetForcePowerById_Success()
        {
            GetEntityById_Success(Generation.ForcePower());
        }

        [Fact]
        public void GetForcePowerByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.ForcePower());
        }

        [Fact]
        public void GetForcePowerByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.ForcePower());
        }

        [Fact]
        public void GetForcePowersWithPaging_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 50, generateMax: true);
            for (var i = 0; i < forcePowers.Count; i++)
            {
                forcePowers[i].Level = 0;
                forcePowers[i].Name = $"ForcePower - {i:00000}";
            }
            GetEntitiesWithPaging_Success(forcePowers);
        }

        [Fact]
        public void GetForcePowersWithSearchingNull_FullList()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters();
            
            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().BeEquivalentTo(forcePowers);
        }

        [Fact]
        public void GetForcePowersWithSearchingName_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            forcePowers[0].Name = "Found";
            forcePowers[1].Name = "IsFound";

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                Search = "found"
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);
        }

        [Fact]
        public void GetForcePowersWithSearchingLevel_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            forcePowers[0].Level = 1;
            forcePowers[1].Level = 1;
            forcePowers[2].Level = 2;

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                Levels = new[] { 1 }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Levels = new[] { 1, 2 };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingAlignment_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            forcePowers[0].Alignment = ForceAlignment.Dark;
            forcePowers[1].Alignment = ForceAlignment.Dark;
            forcePowers[2].Alignment = ForceAlignment.Light;

            for (var i = 3; i < forcePowers.Count; i++)
            {
                forcePowers[i].Alignment = ForceAlignment.Universal;
            }

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                Alignments = new[] { ForceAlignment.Dark }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Alignments = new[] { ForceAlignment.Dark, ForceAlignment.Light };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingCastingPeriod_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            forcePowers[0].CastingPeriod = CastingPeriod.EightHours;
            forcePowers[1].CastingPeriod = CastingPeriod.EightHours;
            forcePowers[2].CastingPeriod = CastingPeriod.Action;

            for (var i = 3; i < forcePowers.Count; i++)
            {
                forcePowers[i].CastingPeriod = CastingPeriod.BonusAction;
            }

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                CastingPeriods = new[] { CastingPeriod.EightHours }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.CastingPeriods = new[] { CastingPeriod.EightHours, CastingPeriod.Action };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingPowerRange_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);

            forcePowers[0].Range = PowerRange.Self;
            forcePowers[1].Range = PowerRange.Self;
            forcePowers[2].Range = PowerRange.FiveHundredFt;

            for (var i = 3; i < forcePowers.Count; i++)
            {
                forcePowers[i].Range = PowerRange.Varies;
            }

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                Ranges = new[] { PowerRange.Self }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Ranges = new[] { PowerRange.Self, PowerRange.FiveHundredFt };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingAll_Success()
        {
            var forcePowers = new List<ForcePower>
            {
                new() { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Varies },
                new() { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.BonusAction, Range = PowerRange.Self },
                new() { Name = "found", Level = 0, Alignment = ForceAlignment.Light, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "found", Level = 1, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "not", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
            };

            foreach (var power in forcePowers)
            {
                power.Id = Guid.NewGuid();
                power.User = MockHttpContext.CurrentUser;
            }

            foreach (var entity in forcePowers)
            {
                _mockRepo.AddEntity(entity);
            }
            _mockRepo.SaveAll();

            var paging = new PagingParameters();
            var searching = new ForcePowerSearchParameters
            {
                Search = "fou",
                Levels = new[] { 0 },
                Alignments = new[] { ForceAlignment.Dark },
                CastingPeriods = new[] { CastingPeriod.Action },
                Ranges = new[] { PowerRange.Self }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(1);
        }
    }
}
