using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
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
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };
            return characterRequest;
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
                Actions = new List<BaseAction> { BaseAction(), BaseAction() },
                Vulnerabilities = DamageTypes(),
                Immunities = DamageTypes(),
                Resistances = DamageTypes(),
                ForcePowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                TechPowerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
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
                HitOption = Faker.Boolean.Random() ? RandomEnum<HitOption>() : null,
                HitDescription = Nonsense(50),
                Alignment = RandomEnum<ForceAlignment>(),
                Potency = Nonsense()
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
                HitOption = Faker.Boolean.Random() ? RandomEnum<HitOption>() : null,
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

        public static DamageType[] DamageTypes()
        {
            var damageTypes = Enum.GetValues<DamageType>().ToList();
            var totalDamageTypes = damageTypes.Count;
            var selections = Faker.RandomNumber.Next(0, totalDamageTypes);
            var randomList = new List<DamageType>();
            for (int i = 0; i < selections; i++)
            {
                var index = Faker.RandomNumber.Next(0, damageTypes.Count - 1);
                randomList.Add(damageTypes[index]);
                damageTypes.RemoveAt(index);
            }
            return randomList.ToArray();
        }

        public static string Nonsense(int size = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, size)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
