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
            var characters = Generation.RandomList(() => Generation.Character(), 50);
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].Name = $"Character - {string.Format("{0:00000}", i)}";
            }
            GetEntitiesWithPaging_Success(characters);
        }
    }
}
