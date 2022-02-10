using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Query;
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
            var searching = new ForcePowerSearchParameters();
            GetEntitiesWithSearching_Success(forcePowers, searching, _ => true);
        }

        [Fact]
        public void GetForcePowersWithSearchingName_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);
            forcePowers[0].Name = "Found";
            forcePowers[1].Name = "IsFound";
            var searching = new ForcePowerSearchParameters
            {
                Search = "found"
            };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Name?.ToLower().Contains("found") == true);
        }

        [Fact]
        public void GetForcePowersWithSearchingLevel_Success()
        {
            var forcePowers = Generation.RandomList(Generation.ForcePower, max: 10, generateMax: true);
            forcePowers[0].Level = 1;
            forcePowers[1].Level = 1;
            forcePowers[2].Level = 2;
            var searching = new ForcePowerSearchParameters
            {
                Levels = new[] { 1 }
            };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Level == 1);
            searching.Levels = new[] { 1, 2 };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Level is 1 or 2);
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
            var searching = new ForcePowerSearchParameters
            {
                Alignments = new[] { ForceAlignment.Dark }
            };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Alignment == ForceAlignment.Dark);
            searching.Alignments = new[] { ForceAlignment.Dark, ForceAlignment.Light };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Alignment is ForceAlignment.Dark or ForceAlignment.Light);
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
            var searching = new ForcePowerSearchParameters
            {
                CastingPeriods = new[] { CastingPeriod.EightHours }
            };

            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.CastingPeriod == CastingPeriod.EightHours);
            searching.CastingPeriods = new[] { CastingPeriod.EightHours, CastingPeriod.Action };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.CastingPeriod is CastingPeriod.EightHours or CastingPeriod.Action);
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
            var searching = new ForcePowerSearchParameters
            {
                Ranges = new[] { PowerRange.Self }
            };

            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Range == PowerRange.Self);
            searching.Ranges = new[] { PowerRange.Self, PowerRange.FiveHundredFt };
            GetEntitiesWithSearching_Success(forcePowers, searching, x => x.Range is PowerRange.Self or PowerRange.FiveHundredFt);
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
            var searching = new ForcePowerSearchParameters
            {
                Search = "fou",
                Levels = new[] { 0 },
                Alignments = new[] { ForceAlignment.Dark },
                CastingPeriods = new[] { CastingPeriod.Action },
                Ranges = new[] { PowerRange.Self }
            };

            GetEntitiesWithSearching_Success(forcePowers, searching, x => 
                x.Name?.ToLower().Contains("fou") == true &&
                x.Level == 0 &&
                x.Alignment == ForceAlignment.Dark &&
                x.CastingPeriod == CastingPeriod.Action &&
                x.Range == PowerRange.Self);
        }
    }
}
