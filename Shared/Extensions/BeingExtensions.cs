using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public static class BeingExtensions
    {
        public static int GetProficiencyBonus(this Being being)
        {
            if (being is Character character)
            {
                return character.Level switch
                {
                    < 5 => 2,
                    < 9 => 3,
                    < 13 => 4,
                    < 17 => 5,
                    _ => 6
                };
            }

            return 0;
        }

        public static int ForceAttackModifier(this Being being, ForceAlignment alignment)
        {
            return alignment switch
            {
                ForceAlignment.Light => being.WisdomBonus + being.GetProficiencyBonus(),
                ForceAlignment.Dark => being.CharismaBonus + being.GetProficiencyBonus(),
                ForceAlignment.Universal => Math.Max(being.WisdomBonus, being.CharismaBonus) + being.GetProficiencyBonus(),
                _ => 0,
            };
        }

        public static int ForceSavingThrow(this Being being, ForceAlignment alignment)
        {
            return 8 + being.ForceAttackModifier(alignment);
        }

        public static int SkillBonus(this Being being, int bonus, bool? proficient)
        {
            return proficient switch
            {
                false => bonus,
                null => bonus + being.GetProficiencyBonus(),
                _ => bonus + 2 * being.GetProficiencyBonus()
            };
        }

        public static int WeaponBonus(this Being being, bool melee, bool damage = false)
        {
            var bonus = melee ? being.StrengthBonus : being.DexterityBonus;
            if (damage)
                return bonus;
            return bonus + being.GetProficiencyBonus();
        }

        public static void StrengthChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var str)) return;
            being.Strength = str;
            being.StrengthBonus = CalculateBonus(str);
        }

        public static void DexterityChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var dex)) return;
            being.Dexterity = dex;
            being.DexterityBonus = CalculateBonus(dex);
        }

        public static void ConstitutionChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var con)) return;
            being.Constitution = con;
            being.ConstitutionBonus = CalculateBonus(con);
        }

        public static void IntelligenceChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var intelligence)) return;
            being.Intelligence = intelligence;
            being.IntelligenceBonus = CalculateBonus(intelligence);
        }

        public static void WisdomChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var wis)) return;
            being.Wisdom = wis;
            being.WisdomBonus = CalculateBonus(wis);
        }

        public static void CharismaChanged(this Being being, string value)
        {
            if (!int.TryParse(value, out var cha)) return;
            being.Charisma = cha;
            being.CharismaBonus = CalculateBonus(cha);
        }

        private static int CalculateBonus(int value)
        {
            var bonus = value / 2;
            return bonus - 5;
        }
    }
}
