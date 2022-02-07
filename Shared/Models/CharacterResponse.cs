using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class CharacterResponse : CharacterRequest, IBeingResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;

        public int GetProficiencyBonus()
        {
            return Level switch
            {
                < 5 => 2,
                < 9 => 3,
                < 13 => 4,
                < 17 => 5,
                _ => 6
            };
        }

        public int SkillBonus(int bonus, bool? proficient)
        {
            return proficient switch
            {
                false => bonus,
                null => bonus + GetProficiencyBonus(),
                _ => bonus + 2 * GetProficiencyBonus()
            };
        }

        public int WeaponBonus(bool melee, bool damage = false)
        {
            var bonus = melee ? StrengthBonus : DexterityBonus;
            if (damage)
                return bonus;
            return bonus + GetProficiencyBonus();
        }


        public int ForceAttackModifier(ForceAlignment alignment)
        {
            return alignment switch
            {
                ForceAlignment.Light => WisdomBonus + GetProficiencyBonus(),
                ForceAlignment.Dark => CharismaBonus + GetProficiencyBonus(),
                ForceAlignment.Universal => Math.Max(WisdomBonus, CharismaBonus) + GetProficiencyBonus(),
                _ => 0,
            };
        }

        public int ForceSavingThrow(ForceAlignment alignment)
        {
            return 8 + ForceAttackModifier(alignment);
        }
    }
}
