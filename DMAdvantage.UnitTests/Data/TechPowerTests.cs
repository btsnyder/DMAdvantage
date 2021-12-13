﻿using TestEngineering;
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
            var techPowers = Generation.RandomList(() => Generation.TechPower(), 50);
            for (int i = 0; i < techPowers.Count; i++)
            {
                techPowers[i].Level = 0;
                techPowers[i].Name = $"TechPower - {string.Format("{0:00000}", i)}";
            }
            GetEntitiesWithPaging_Success(techPowers);
        }
    }
}
