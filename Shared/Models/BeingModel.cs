using System.ComponentModel.DataAnnotations;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class BeingModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int HitPoints { get; set; }
        [Required]
        public int ArmorClass { get; set; }
        [Required]
        public string? Speed { get; set; }
        [Required]
        public int Strength { get; set; }
        [Required]
        public int StrengthBonus { get; set; }
        [Required]
        public int Dexterity { get; set; }
        [Required]
        public int DexterityBonus { get; set; }
        [Required]
        public int Constitution { get; set; }
        [Required]
        public int ConstitutionBonus { get; set; }
        [Required]
        public int Intelligence { get; set; }
        [Required]
        public int IntelligenceBonus { get; set; }
        [Required]
        public int Wisdom { get; set; }
        [Required]
        public int WisdomBonus { get; set; }
        [Required]
        public int Charisma { get; set; }
        [Required]
        public int CharismaBonus { get; set; }
        public List<Guid> ForcePowerIds { get; set; } = new();
        public int ForcePoints { get; set; }
        public int TotalForcePowers { get; set; }
        public int MaxForcePowerLevel { get; set; }
        public List<Guid> TechPowerIds { get; set; } = new();
        public int TechPoints { get; set; }
        public int GetProficiencyBonus() => 0;
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
