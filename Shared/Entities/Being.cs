namespace DMAdvantage.Shared.Entities
{
    public abstract class Being : BaseEntity, INamedEntity
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
        public List<Guid> ForcePowerIds { get; set; } = new();
        public int ForcePoints { get; set; }
        public List<Guid> TechPowerIds { get; set; } = new();
        public int TechPoints { get; set; }
    }
}
