using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class CreatureTests : BaseEntityTests
    {
        [Fact]
        public void GetAllCreatures_Success()
        {
            GetAllEntities_Success(Generation.Creature());
        }

        [Fact]
        public void GetAllCreaturesFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.Creature());
        }

        [Fact]
        public void GetCreatureById_Success()
        {
            GetEntityById_Success(Generation.Creature());
        }

        [Fact]
        public void GetCreatureByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.Creature());
        }

        [Fact]
        public void GetCreatureByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.Creature());
        }

        [Fact]
        public void GetCreatureWithPaging_Success()
        {
            var creatures = Generation.RandomList(Generation.Creature, max: 50, generateMax: true);
            for (var i = 0; i < creatures.Count; i++)
            {
                creatures[i].Name = $"Creature - {i:00000}";
            }
            GetEntitiesWithPaging_Success(creatures);
        }

        [Fact]
        public void GetCreatureWithSearching_Success()
        {
            var creatures = Generation.RandomList(Generation.Creature, max: 50, generateMax: true);
            foreach (var creature in creatures)
            {
                if (Faker.Boolean.Random())
                    creature.Name = "FOUND";
            }
            var search = new NamedSearchParameters<Creature> { Search = "FOUND" };
            GetEntitiesWithSearching_Success(creatures, search, x => x.Name?.ToLower().Contains("found") == true);
        }
    }
}
