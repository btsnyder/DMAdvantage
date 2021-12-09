﻿using TestEngineering;
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
    }
}