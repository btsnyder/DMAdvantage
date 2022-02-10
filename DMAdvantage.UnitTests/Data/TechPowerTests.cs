using System;
using System.Collections.Generic;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Query;
using TestEngineering;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class TechPowerTests : BaseEntityTests
    {
        [Fact]
        public void GetAllTechPowers_Success()
        {
            GetAllEntities_Success(Generation.TechPower());
        }

        [Fact]
        public void GetAllTechPowersFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.TechPower());
        }

        [Fact]
        public void GetTechPowerById_Success()
        {
            GetEntityById_Success(Generation.TechPower());
        }

        [Fact]
        public void GetTechPowerByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.TechPower());
        }

        [Fact]
        public void GetTechPowerByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.TechPower());
        }

        [Fact]
        public void GetTechPowerWithPaging_Success()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 50, generateMax: true);
            for (var i = 0; i < techPowers.Count; i++)
            {
                techPowers[i].Level = 0;
                techPowers[i].Name = $"TechPower - {i:00000}";
            }
            GetEntitiesWithPaging_Success(techPowers);
        }

        [Fact]
        public void GetTechPowersWithSearchingNull_FullList()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 10, generateMax: true);
            var searching = new TechPowerSearchParameters();
            GetEntitiesWithSearching_Success(techPowers, searching, _ => true);
        }

        [Fact]
        public void GetTechPowersWithSearchingName_Success()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 10, generateMax: true);
            techPowers[0].Name = "Found";
            techPowers[1].Name = "IsFound";
            var searching = new TechPowerSearchParameters
            {
                Search = "found"
            };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.Name?.ToLower().Contains("found") == true);
        }

        [Fact]
        public void GetTechPowersWithSearchingLevel_Success()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 10, generateMax: true);
            techPowers[0].Level = 1;
            techPowers[1].Level = 1;
            techPowers[2].Level = 2;
            var searching = new TechPowerSearchParameters
            {
                Levels = new[] { 1 }
            };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.Level == 1);
            searching.Levels = new[] { 1, 2 };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.Level is 1 or 2);
        }

        [Fact]
        public void GetTechPowersWithSearchingCastingPeriod_Success()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 10, generateMax: true);
            techPowers[0].CastingPeriod = CastingPeriod.EightHours;
            techPowers[1].CastingPeriod = CastingPeriod.EightHours;
            techPowers[2].CastingPeriod = CastingPeriod.Action;
            for (var i = 3; i < techPowers.Count; i++)
            {
                techPowers[i].CastingPeriod = CastingPeriod.BonusAction;
            }
            var searching = new TechPowerSearchParameters
            {
                CastingPeriods = new[] { CastingPeriod.EightHours }
            };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.CastingPeriod == CastingPeriod.EightHours);
            searching.CastingPeriods = new[] { CastingPeriod.EightHours, CastingPeriod.Action };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.CastingPeriod is CastingPeriod.EightHours or CastingPeriod.Action);
        }

        [Fact]
        public void GetTechPowersWithSearchingPowerRange_Success()
        {
            var techPowers = Generation.RandomList(Generation.TechPower, max: 10, generateMax: true);
            techPowers[0].Range = PowerRange.Self;
            techPowers[1].Range = PowerRange.Self;
            techPowers[2].Range = PowerRange.FiveHundredFt;
            for (var i = 3; i < techPowers.Count; i++)
            {
                techPowers[i].Range = PowerRange.Varies;
            }
            var searching = new TechPowerSearchParameters
            {
                Ranges = new[] { PowerRange.Self }
            };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.Range == PowerRange.Self);
            searching.Ranges = new[] { PowerRange.Self, PowerRange.FiveHundredFt };
            GetEntitiesWithSearching_Success(techPowers, searching, x => x.Range is PowerRange.Self or PowerRange.FiveHundredFt);
        }

        [Fact]
        public void GetTechPowersWithSearchingAll_Success()
        {
            var techPowers = new List<TechPower>
            {
                new() { Name = "found", Level = 0, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "found", Level = 0, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Varies },
                new() { Name = "found", Level = 0, CastingPeriod = CastingPeriod.BonusAction, Range = PowerRange.Self },
                new() { Name = "found", Level = 0, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "found", Level = 1, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
                new() { Name = "not", Level = 0, CastingPeriod = CastingPeriod.Action, Range = PowerRange.Self },
            };

            foreach (var power in techPowers)
            {
                power.Id = Guid.NewGuid();
                power.User = MockHttpContext.CurrentUser;
            }

            var searching = new TechPowerSearchParameters
            {
                Search = "fou",
                Levels = new[] { 0 },
                CastingPeriods = new[] { CastingPeriod.Action },
                Ranges = new[] { PowerRange.Self }
            };

            GetEntitiesWithSearching_Success(techPowers, searching, x => 
                x.Name!.ToLower().Contains("fou") &&
                x.Level == 0 &&
                x.CastingPeriod == CastingPeriod.Action &&
                x.Range == PowerRange.Self);
        }
    }
}
