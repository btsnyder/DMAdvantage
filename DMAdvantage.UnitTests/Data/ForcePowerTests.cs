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
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 50, true);
            for (int i = 0; i < forcePowers.Count; i++)
            {
                forcePowers[i].Level = 0;
                forcePowers[i].Name = $"ForcePower - {string.Format("{0:00000}", i)}";
            }
            GetEntitiesWithPaging_Success(forcePowers);
        }

        [Fact]
        public void GetForcePowersWithSearchingNull_FullList()
        {
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

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
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

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
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

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
                Levels = new int[] { 1 }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Levels = new int[] { 1, 2 };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingAlignment_Success()
        {
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

            forcePowers[0].Alignment = ForceAlignment.Dark;
            forcePowers[1].Alignment = ForceAlignment.Dark;
            forcePowers[2].Alignment = ForceAlignment.Light;

            for (int i = 3; i < forcePowers.Count; i++)
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
                Alignments = new ForceAlignment[] { ForceAlignment.Dark }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Alignments = new ForceAlignment[] { ForceAlignment.Dark, ForceAlignment.Light };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingCastingPeriod_Success()
        {
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

            forcePowers[0].CastingPeriod = CastingPeriod.EightHours;
            forcePowers[1].CastingPeriod = CastingPeriod.EightHours;
            forcePowers[2].CastingPeriod = CastingPeriod.Action;

            for (int i = 3; i < forcePowers.Count; i++)
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
                CastingPeriods = new CastingPeriod[] { CastingPeriod.EightHours }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.CastingPeriods = new CastingPeriod[] { CastingPeriod.EightHours, CastingPeriod.Action };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingPowerRange_Success()
        {
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 10, true);

            forcePowers[0].Range = PowerRange.Self;
            forcePowers[1].Range = PowerRange.Self;
            forcePowers[2].Range = PowerRange.FiveHundredFt;

            for (int i = 3; i < forcePowers.Count; i++)
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
                Ranges = new PowerRange[] { PowerRange.Self }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(2);

            searching.Ranges = new PowerRange[] { PowerRange.Self, PowerRange.FiveHundredFt };
            pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(3);
        }

        [Fact]
        public void GetForcePowersWithSearchingAll_Success()
        {
            var forcePowers = new List<ForcePower>
            {
                new ForcePower { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new ForcePower { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Varies },
                new ForcePower { Name = "found", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.BonusAction, Range = PowerRange.Self },
                new ForcePower { Name = "found", Level = 0, Alignment = ForceAlignment.Light, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new ForcePower { Name = "found", Level = 1, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new ForcePower { Name = "not", Level = 0, Alignment = ForceAlignment.Dark, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
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
                Levels = new int[] { 0 },
                Alignments = new ForceAlignment[] { ForceAlignment.Dark },
                CastingPeriods = new CastingPeriod[] { CastingPeriod.Action },
                Ranges = new PowerRange[] { PowerRange.Self }
            };

            var pagedEntities = _mockRepo.GetAllEntities(forcePowers[0].User?.UserName ?? string.Empty, paging, searching);
            pagedEntities.Should().NotBeNull();
            pagedEntities.Should().HaveCount(1);
        }
    }
}
