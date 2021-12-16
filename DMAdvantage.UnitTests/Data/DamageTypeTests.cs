using TestEngineering;
using Xunit;

namespace DMAdvantage.UnitTests.Data
{
    public class DamageTypeTests : BaseEntityTests
    {
        [Fact]
        public void GetAllDamageTypes_Success()
        {
            GetAllEntities_Success(Generation.DamageType());
        }

        [Fact]
        public void GetAllDamageTypesFromDifferentUser_EmptyList()
        {
            GetAllEntitiesFromDifferentUser_EmptyList(Generation.DamageType());
        }

        [Fact]
        public void GetDamageTypeById_Success()
        {
            GetEntityById_Success(Generation.DamageType());
        }

        [Fact]
        public void GetDamageTypeByIdFromDifferentUser_Null()
        {
            GetEntityByIdFromDifferentUser_Null(Generation.DamageType());
        }

        [Fact]
        public void GetDamageTypeByBadId_Null()
        {
            GetEntityByBadId_Null(Generation.DamageType());
        }

        [Fact]
        public void GetDamageTypeWithPaging_Success()
        {
            var damageTypes = Generation.RandomList(() => Generation.DamageType(), 50, true);
            for (int i = 0; i < damageTypes.Count; i++)
            {
                damageTypes[i].Name = $"DamageType - {string.Format("{0:00000}", i)}";
            }
            GetEntitiesWithPaging_Success(damageTypes);
        }
    }
}
