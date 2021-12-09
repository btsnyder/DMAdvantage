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
    }
}
