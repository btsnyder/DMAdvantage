using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class CharacterResponse : CharacterRequest, IBeingResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;

        public int GetProficiencyBonus()
        {
            if (Level < 5)
                return 2;
            if (Level < 9)
                return 3;
            if (Level < 13)
                return 4;
            if (Level < 17)
                return 5;
            return 6;
        }

        public int SkillBonus(int bonus, bool? proficient)
        {
            if (proficient == false)
                return bonus;
            if (proficient == null)
                return bonus + GetProficiencyBonus();
            return bonus + 2 * GetProficiencyBonus();
        }

        public int WeaponBonus(bool melee, bool damage = false)
        {
            int bonus;
            if (melee)
                bonus = StrengthBonus;
            else
                bonus = DexterityBonus;
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
