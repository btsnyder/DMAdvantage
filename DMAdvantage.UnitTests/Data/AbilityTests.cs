using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class AbilityTests : BaseEntityTests
    {
        [Fact]
        public void GetAllAbilities_Success()
        {
            GetAllEntities_Success(Generation.Ability());
        }

        [Fact]
        public void GetAllAbilitiesFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.Ability());
        }

        [Fact]
        public void GetAbilityById_Success()
        {
            GetEntityById_Success(Generation.Ability());
        }

        [Fact]
        public void GetAbilityByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.Ability());
        }

        [Fact]
        public void GetAbilityByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.Ability());
        }

        [Fact]
        public void GetAbilityWithPaging_Success()
        {
            var abilities = Generation.RandomList(Generation.Ability, max: 50, generateMax: true);
            for (var i = 0; i < abilities.Count; i++)
            {
                abilities[i].Name = $"Ability - {i:00000}";
            }
            GetEntitiesWithPaging_Success(abilities);
        }

        [Fact]
        public void GetAbilityWithSearching_Success()
        {
            var abilities = Generation.RandomList(Generation.Ability, max: 50, generateMax: true);
            foreach (var ability in abilities)
            {
                if (Faker.Boolean.Random())
                    ability.Name = "FOUND";
            }
            var search = new NamedSearchParameters<Ability> { Search = "FOUND" };
            GetEntitiesWithSearching_Success(abilities, search, x => x.Name?.ToLower().Contains("found") == true);
        }
    }
}
