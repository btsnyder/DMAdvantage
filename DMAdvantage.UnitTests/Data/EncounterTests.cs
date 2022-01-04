using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TestEngineering;
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

        [Fact]
        public void GetEncounterWithPaging_Success()
        {
            var encounters = Generation.RandomList(() => GenerateEncounter(), 50, true);
            encounters = encounters.OrderBy(x => x.Id).ToList();
            GetEntitiesWithPaging_Success(encounters);
        }

        private static Encounter GenerateEncounter()
        {
            return new Encounter
            {
                Id = Guid.NewGuid(),
                DataCache = JsonSerializer.Serialize(new List<InitativeData> 
                { 
                    new InitativeData { BeingId = Guid.NewGuid() },
                    new InitativeData { BeingId = Guid.NewGuid() }
                }),
                User = MockHttpContext.CurrentUser
            };
        }
    }
}
