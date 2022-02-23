using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public interface IBeingResponse : IEntityResponse
    {
        public string? Name { get; set; }
        public int HitPoints { get; set; }
        public int ArmorClass { get; set; }
        public string? Speed { get; set; }
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
        public List<Guid> ForcePowerIds { get; set; }
        public int ForcePoints { get; set; }
        public List<Guid> TechPowerIds { get; set; }
        public int TechPoints { get; set; }
        public int GetProficiencyBonus();
        public int ForceAttackModifier(ForceAlignment alignment);
        public int ForceSavingThrow(ForceAlignment alignment);
    }
}
