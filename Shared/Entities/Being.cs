using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public abstract class Being : BaseEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int ArmorClass { get; set; }
        public string Speed { get; set; }
        public int Strength { get; set; }
        public int StrengthBonus { get; set; }
        public int Dexterity { get; set; }
        public int DexterityBonus { get; set; }
        public int Constitution { get; set; }
        public int ConstitutionBonus { get; set; }
        public int Intelligence { get; set; }
        public int IntelligenceBonus { get; set; }
        public int Wisdom { get; set; }
        public int WisdomBonus { get; set; }
        public int Charisma { get; set; }
        public int CharismaBonus { get; set; }
        public ICollection<ForcePower> ForcePowers { get; set; } = new List<ForcePower>();
        public int ForcePoints { get; set; }
        public int TotalForcePowers { get; set; }
        public int MaxForcePowerLevel { get; set; }
        public ICollection<TechPower> TechPowers { get; set; } = new List<TechPower>();
        public int TechPoints { get; set; }

        public new string Display => Name;
    }
}
