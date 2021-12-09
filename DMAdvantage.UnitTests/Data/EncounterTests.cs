using DMAdvantage.Shared.Entities;
using System;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class EncounterTests : BaseEntityTests
    {
        [Fact]
        public void GetAllEncounters_Success()
        {
            GetAllEntities_Success(GenerateEncounter());
        }

        [Fact]
        public void GetAllEncountersFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(GenerateEncounter());
        }

        [Fact]
        public void GetEncounterById_Success()
        {
            GetEntityById_Success(GenerateEncounter());
        }

        [Fact]
        public void GetEncounterByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(GenerateEncounter());
        }

        [Fact]
        public void GetEncounterByBadId_Null()
        {
            GetEntityByBadId_Null(GenerateEncounter());
        }

        private static Encounter GenerateEncounter()
        {
            return new Encounter
            {
                Id = Guid.NewGuid(),
                CharacterIds = new() { Guid.NewGuid() },
                CreatureIds = new() { Guid.NewGuid() },
                User = MockHttpContext.CurrentUser
            };
        }
    }
}
