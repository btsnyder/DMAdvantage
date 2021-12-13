using TestEngineering;
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
        public void GetForcePowerWithPaging_Success()
        {
            var forcePowers = Generation.RandomList(() => Generation.ForcePower(), 50);
            for (int i = 0; i < forcePowers.Count; i++)
            {
                forcePowers[i].Level = 0;
                forcePowers[i].Name = $"ForcePower - {string.Format("{0:00000}", i)}";
            }
            GetEntitiesWithPaging_Success(forcePowers);
        }
    }
}
