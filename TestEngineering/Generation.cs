using System.Text.Json;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;

namespace TestEngineering
{
    public static class Generation
    {
        public static Character Character()
        {
            return new Character
            {
                Name = Faker.Name.FullName(),
                HitPoints = Faker.RandomNumber.Next(),
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = Faker.Lorem.GetFirstWord(),
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
            };
        }

        public static Weapon Weapon()
        {
            return new Weapon
            {
                Name = Faker.Lorem.GetFirstWord(),
                Type = RandomEnum<WeaponType>(),
                Melee = Faker.Boolean.Random(),
                Description = Faker.Lorem.Paragraph(),
                Damage = Faker.Lorem.GetFirstWord(),
                DamageType = RandomEnum<DamageType>(),
            };
        }

        public static WeaponProperty WeaponProperty()
        {
            return new WeaponProperty
            {
                Description = Faker.Lorem.Paragraph(),
                Name = Faker.Lorem.Sentence(),
                Modifier = Faker.Lorem.GetFirstWord()
            };
        }

        public static Equipment Equipment()
        {
            return new Equipment
            {
                Description = Faker.Lorem.Paragraph(),
                Name = Faker.Lorem.Sentence(),
                Category = RandomEnum<EquipmentCategory>()
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

        public static Creature Creature()
        {
            var creatureRequest = new Creature
            {
                Name = Faker.Name.FullName(),
                HitPoints = Faker.RandomNumber.Next(),
                ArmorClass = Faker.RandomNumber.Next(),
                Speed = Faker.Lorem.GetFirstWord(),
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
                Vulnerabilities = RandomList(RandomEnum<DamageType>),
                Immunities = RandomList(RandomEnum<DamageType>),
                Resistances = RandomList(RandomEnum<DamageType>),
                ForcePoints = Faker.RandomNumber.Next(),
                TotalForcePowers = Faker.RandomNumber.Next(),
                MaxForcePowerLevel = Faker.RandomNumber.Next(),
                TechPoints = Faker.RandomNumber.Next(),
            };
            return creatureRequest;
        }

        public static ForcePower ForcePower()
        {
            var forcePowerRequest = new ForcePower
            {
                Name = Faker.Lorem.Sentence(),
                Description = Faker.Lorem.Paragraph(),
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = RandomEnum<CastingPeriod>(),
                CastingDescription = Faker.Lorem.Paragraph(),
                Range = RandomEnum<PowerRange>(),
                RangeDescription = Faker.Lorem.Paragraph(),
                Duration = Faker.Lorem.GetFirstWord(),
                Concentration = Faker.Boolean.Random(),
                HitOption = RandomEnum<HitOption>(),
                HitDescription = Faker.Lorem.Paragraph(),
                Alignment = RandomEnum<ForceAlignment>(),
                Potency = Faker.Lorem.Sentence(),
                PrerequisiteId = Guid.NewGuid(),
            };

            return forcePowerRequest;
        }

        public static TechPower TechPower()
        {
            var techPowerRequest = new TechPower
            {
                Name = Faker.Lorem.Sentence(),
                Description = Faker.Lorem.Paragraph(),
                Level = Faker.RandomNumber.Next(),
                CastingPeriod = RandomEnum<CastingPeriod>(),
                CastingDescription = Faker.Lorem.Paragraph(),
                Range = RandomEnum<PowerRange>(),
                RangeDescription = Faker.Lorem.Paragraph(),
                Duration = Faker.Lorem.GetFirstWord(),
                Concentration = Faker.Boolean.Random(),
                HitOption = RandomEnum<HitOption>(),
                HitDescription = Faker.Lorem.Paragraph(),
                Overcharge = Faker.Lorem.Sentence(),
            };

            return techPowerRequest;
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
                Name = Faker.Lorem.Sentence(),
                Description = Faker.Lorem.Paragraph(),
                Hit = Faker.Lorem.GetFirstWord(),
                Range = Faker.Lorem.GetFirstWord(),
                Damage = Faker.Lorem.GetFirstWord()
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

        public static InitativeData InitativeData()
        {
            var data = new InitativeData
            {
                Initative = Faker.RandomNumber.Next(0, 20),
                CurrentHP = Faker.RandomNumber.Next(0, 200),
                CurrentFP = Faker.RandomNumber.Next(0, 20),
                CurrentTP = Faker.RandomNumber.Next(0, 20)
            };
            if (Faker.Boolean.Random())
                data.Creature = Creature();
            else
                data.Character = Character();
            return data;
        }

        public static Encounter Encounter()
        {
            var mockInitativeData = RandomList(InitativeData, 2);
            var mockConcentration = new Dictionary<string, Guid>();
            for (var i = 0; i < Faker.RandomNumber.Next(1, mockInitativeData.Count); i++)
            {
                mockConcentration[Faker.Lorem.GetFirstWord()] = Guid.NewGuid();
            }

            return new Encounter
            {
                Name = Faker.Lorem.Sentence(),
                CurrentPlayer = mockInitativeData[Faker.RandomNumber.Next(0, mockInitativeData.Count - 1)].Being?.Id ?? Guid.Empty,
                InitativeData = mockInitativeData,
                ConcentrationCache = JsonSerializer.Serialize(mockConcentration),
            };
        }

        public static Ability Ability()
        {
            return new Ability
            {
                Name = Faker.Name.FullName(),
                Description = Faker.Lorem.Paragraph(),
            };
        }

        public static DMClass DMClass()
        {
            return new DMClass
            {
                Name = Faker.Name.FullName(),
                HitDice = Faker.RandomNumber.Next(),
            };
        }
    }
}
