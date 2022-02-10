using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class CharacterTests : BaseEntityTests
    {
        [Fact]
        public void GetAllCharacters_Success()
        {
            GetAllEntities_Success(Generation.Character());
        }

        [Fact]
        public void GetAllCharactersFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.Character());
        }

        [Fact]
        public void GetCharacterById_Success()
        {
            GetEntityById_Success(Generation.Character());
        }

        [Fact]
        public void GetCharacterByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.Character());
        }

        [Fact]
        public void GetCharacterByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.Character());
        }

        [Fact]
        public void GetCharacterWithPaging_Success()
        {
            var characters = Generation.RandomList(Generation.Character, max: 50, generateMax: true);
            for (var i = 0; i < characters.Count; i++)
            {
                characters[i].Name = $"Character - {i:00000}";
            }
            GetEntitiesWithPaging_Success(characters);
        }

        [Fact]
        public void GetCharacterWithSearching_Success()
        {
            var characters = Generation.RandomList(Generation.Character, max: 50, generateMax: true);
            foreach (var character in characters)
            {
                if (Faker.Boolean.Random())
                    character.Name = "FOUND";
            }
            var search = new NamedSearchParameters<Character> { Search = "FOUND" };
            GetEntitiesWithSearching_Success(characters, search, x => x.Name?.ToLower().Contains("found") == true);
        }
    }
}
