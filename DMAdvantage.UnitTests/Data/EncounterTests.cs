using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class EncounterTests : BaseEntityTests
    {
        [Fact]
        public void GetAllEncounters_Success()
        {
            GetAllEntities_Success(Generation.Encounter());
        }

        [Fact]
        public void GetAllEncountersFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.Encounter());
        }

        [Fact]
        public void GetEncounterById_Success()
        {
            GetEntityById_Success(Generation.Encounter());
        }

        [Fact]
        public void GetEncounterByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.Encounter());
        }

        [Fact]
        public void GetEncounterByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.Encounter());
        }

        [Fact]
        public void GetEncounterWithPaging_Success()
        {
            var encounters = Generation.RandomList(Generation.Encounter, max: 50, generateMax: true);
            for (var i = 0; i < encounters.Count; i++)
            {
                encounters[i].Name = $"Encounter - {i:00000}";
            }
            GetEntitiesWithPaging_Success(encounters);
        }

        [Fact]
        public void GetEncounterWithSearching_Success()
        {
            var encounters = Generation.RandomList(Generation.Encounter, max: 50, generateMax: true);
            foreach (var encounter in encounters)
            {
                if (Faker.Boolean.Random())
                    encounter.Name = "FOUND";
            }
            var search = new NamedSearchParameters<Encounter> { Search = "FOUND" };
            GetEntitiesWithSearching_Success(encounters, search, x => x.Name?.ToLower().Contains("found") == true);
        }
    }
}
