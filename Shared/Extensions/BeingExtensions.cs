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
    }
}
