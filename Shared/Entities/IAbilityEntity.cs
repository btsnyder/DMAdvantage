namespace DMAdvantage.Shared.Entities
{
    public interface IAbilityEntity
    {
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
    }
}
