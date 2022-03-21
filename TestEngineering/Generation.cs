﻿using System.Text.Json;
using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Moq;
using TestEngineering.Mocks;

namespace TestEngineering
{
    public static class Generation
    {
        private static readonly Random _random = new();
        private static readonly Mapper _mapper;

        static Generation()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
        }

        public static CharacterRequest CharacterRequest()
        {
            var characterRequest = new CharacterRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = Faker.RandomNumber.Next(),
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = Nonsense(),
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = Faker.RandomNumber.Next(),
                DexterityBonus = Faker.RandomNumber.Next(),
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = Faker.RandomNumber.Next(),
                IntelligenceBonus = Faker.RandomNumber.Next(),
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = Faker.RandomNumber.Next(),
                CharismaBonus = Faker.RandomNumber.Next(),
                PlayerName = Faker.Name.FullName(),
                Level = Faker.RandomNumber.Next(),
                ForcePoints = Faker.RandomNumber.Next(),
                TotalForcePowers = Faker.RandomNumber.Next(),
                MaxForcePowerLevel= Faker.RandomNumber.Next(),
                TechPowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPoints = Faker.RandomNumber.Next(),
                StrengthSave = NullableBoolean(),
                DexteritySave = NullableBoolean(),
                ConstitutionSave = NullableBoolean(),
                IntelligenceSave = NullableBoolean(),
                WisdomSave = NullableBoolean(),
                CharismaSave = NullableBoolean(),
                Athletics = NullableBoolean(),
                Acrobatics = NullableBoolean(),
                SleightOfHand = NullableBoolean(),
                Stealth = NullableBoolean(),
                Investigation = NullableBoolean(),
                Lore = NullableBoolean(),
                Nature = NullableBoolean(),
                Piloting = NullableBoolean(),
                Technology = NullableBoolean(),
                AnimalHandling = NullableBoolean(),
                Insight = NullableBoolean(),
                Medicine = NullableBoolean(),
                Perception = NullableBoolean(),
                Survival = NullableBoolean(),
                Deception = NullableBoolean(),
                Intimidation = NullableBoolean(),
                Performance = NullableBoolean(),
                Persuasion = NullableBoolean(),
                Weapons = RandomList(Weapon)
            };
            return characterRequest;
        }

        public static Weapon Weapon()
        {
            return new Weapon
            {
                Name = Nonsense(),
                Type = RandomEnum<WeaponType>(),
                Melee = Faker.Boolean.Random(),
                Description = Nonsense(),
                Damage = Nonsense(),
                DamageType = RandomEnum<DamageType>(),
                Properties = RandomList(() => Nonsense()).ToArray(),
                PropertyDescriptions = RandomList(WeaponDescription)
            };
        }

        public static WeaponDescription WeaponDescription()
        {
            return new WeaponDescription
            {
                Description = Nonsense(),
                Name = Nonsense()
            };
        }

        public static bool? NullableBoolean()
        {
            var i = Faker.RandomNumber.Next(0, 2);
            return i switch
            {
                0 => null,
                1 => true,
                _ => false,
            };
        }

        public static Character Character()
        {
            var character = _mapper.Map<Character>(CharacterRequest());
            character.Id = Guid.NewGuid();
            character.User = MockHttpContext.CurrentUser;
            return character;
        }

        public static CreatureRequest CreatureRequest()
        {
            var creatureRequest = new CreatureRequest
            {
                Name = Faker.Name.FullName(),
                HitPoints = Faker.RandomNumber.Next(),
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = Nonsense(),
                Strength = Faker.RandomNumber.Next(),
                StrengthBonus = Faker.RandomNumber.Next(),
                Dexterity = Faker.RandomNumber.Next(),
                DexterityBonus = Faker.RandomNumber.Next(),
                Constitution = Faker.RandomNumber.Next(),
                ConstitutionBonus = Faker.RandomNumber.Next(),
                Intelligence = Faker.RandomNumber.Next(),
                IntelligenceBonus = Faker.RandomNumber.Next(),
                Wisdom = Faker.RandomNumber.Next(),
                WisdomBonus = Faker.RandomNumber.Next(),
                Charisma = Faker.RandomNumber.Next(),
                CharismaBonus = Faker.RandomNumber.Next(),
                ChallengeRating = Faker.RandomNumber.Next(),
                Actions = RandomList(BaseAction),
                Vulnerabilities = RandomList(RandomEnum<DamageType>),
                Immunities = RandomList(() => RandomEnum<DamageType>().ToString()),
                Resistances = RandomList(() => RandomEnum<DamageType>().ToString()),
                ForcePoints = Faker.RandomNumber.Next(),
                TotalForcePowers = Faker.RandomNumber.Next(),
                MaxForcePowerLevel = Faker.RandomNumber.Next(),
                TechPowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPoints = Faker.RandomNumber.Next(),
            };
            return creatureRequest;
        }

        public static Creature Creature()
        {
            var creature = _mapper.Map<Creature>(CreatureRequest());
            creature.Id = Guid.NewGuid();
            creature.User = MockHttpContext.CurrentUser;
            return creature;
        }

        public static ForcePowerRequest ForcePowerRequest()
        {
            var forcePowerRequest = new ForcePowerRequest
            {
                Name = Nonsense(),
                Description = Nonsense(50),
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = RandomEnum<CastingPeriod>(),
                CastingDescription = Nonsense(50),
                Range = RandomEnum<PowerRange>(),
                RangeDescription = Nonsense(50),
                Duration = Nonsense(),
                Concentration = Faker.Boolean.Random(),
                HitOption = RandomEnum<HitOption>(),
                HitDescription = Nonsense(50),
                Alignment = RandomEnum<ForceAlignment>(),
                Potency = Nonsense(),
                PrerequisiteId = Guid.NewGuid()
            };

            return forcePowerRequest;
        }

        public static ForcePower ForcePower()
        {
            var forcePower = _mapper.Map<ForcePower>(ForcePowerRequest());
            forcePower.Id = Guid.NewGuid();
            forcePower.User = MockHttpContext.CurrentUser;
            return forcePower;
        }

        public static TechPowerRequest TechPowerRequest()
        {
            var techPowerRequest = new TechPowerRequest
            {
                Name = Nonsense(),
                Description = Nonsense(50),
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = RandomEnum<CastingPeriod>(),
                CastingDescription = Nonsense(50),
                Range = RandomEnum<PowerRange>(),
                RangeDescription = Nonsense(50),
                Duration = Nonsense(),
                Concentration = Faker.Boolean.Random(),
                HitOption = RandomEnum<HitOption>(),
                HitDescription = Nonsense(50),
                Overcharge = Nonsense()
            };

            return techPowerRequest;
        }

        public static TechPower TechPower()
        {
            var techPower = _mapper.Map<TechPower>(TechPowerRequest());
            techPower.Id = Guid.NewGuid();
            techPower.User = MockHttpContext.CurrentUser;
            return techPower;
        }

        public static T RandomEnum<T>() where T : struct, Enum
        {
            var enumValues = Enum.GetValues<T>();
            return enumValues[Faker.RandomNumber.Next(0, enumValues.Length - 1)];
        }

        public static BaseAction BaseAction()
        {
            return new BaseAction
            {
                Name = Nonsense(),
                Description = Nonsense(50),
                Hit = Nonsense(),
                Range = Nonsense(),
                Damage = Nonsense()
            };
        }

        public static List<T> RandomList<T>(Func<T> creation, int min = 0, int max = 5, bool generateMax = false)
        {
            var list = new List<T>();
            var count = !generateMax ? Faker.RandomNumber.Next(min, max) : max;
            for (var i = 0; i < count; i++)
            {
                list.Add(creation.Invoke());
            }
            return list;
        }

        public static string Nonsense(int size = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
            return new string(Enumerable.Repeat(chars, size)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static InitativeData InitativeData()
        {
            return new InitativeData
            {
                Initative = Faker.RandomNumber.Next(0, 20),
                BeingId = Guid.NewGuid(),
                CurrentHP = Faker.RandomNumber.Next(0, 200),
                CurrentFP = Faker.RandomNumber.Next(0, 20),
                CurrentTP = Faker.RandomNumber.Next(0, 20)
            };
        }

        public static Encounter Encounter()
        {
            var encounter = _mapper.Map<Encounter>(EncounterRequest());
            encounter.Id = Guid.NewGuid();
            encounter.User = MockHttpContext.CurrentUser;
            return encounter;
        }

        public static EncounterRequest EncounterRequest()
        {
            var mockInitativeData = RandomList(InitativeData, 2);
            var mockConcentration = new Dictionary<string, Guid>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, mockInitativeData.Count); i++)
            {
                mockConcentration[Nonsense()] = Guid.NewGuid();
            }

            return new EncounterRequest
            {
                Name = Nonsense(),
                CurrentPlayer = mockInitativeData[Faker.RandomNumber.Next(0, mockInitativeData.Count - 1)].BeingId,
                Data = mockInitativeData,
                ConcentrationPowers = mockConcentration
            };
        }

        public static AbilityRequest AbilityRequest()
        {
            return new AbilityRequest
            {
                Name = Faker.Name.FullName(),
                Description = Nonsense()
            };
        }

        public static Ability Ability()
        {
            var ability = _mapper.Map<Ability>(AbilityRequest());
            ability.Id = Guid.NewGuid();
            ability.User = MockHttpContext.CurrentUser;
            return ability;
        }

        public static DMClassRequest DMClassRequest()
        {
            return new DMClassRequest
            {
                Name = Faker.Name.FullName(),
                HitDice = Faker.RandomNumber.Next()
            };
        }

        public static DMClass DMClass()
        {
            var dmclass = _mapper.Map<DMClass>(DMClassRequest());
            dmclass.Id = Guid.NewGuid();
            dmclass.User = MockHttpContext.CurrentUser;
            return dmclass;
        }
    }
}
