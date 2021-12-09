using TestEngineering;
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
    }
}
