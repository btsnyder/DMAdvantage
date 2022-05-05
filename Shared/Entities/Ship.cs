namespace DMAdvantage.Shared.Entities
{
    public abstract class Ship : BaseEntity, IAbilityEntity
    {
        public int HullPoints { get; set; }
        public int HullHitDice { get; set; }
        public int HullHitDiceNumber { get; set; }
        public int ShieldPoints { get; set; }
        public int ShieldHitDice{ get; set; }
        public int ShieldHitDiceNumber { get; set; }
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

        public ICollection<ShipWeapon> Weapons { get; set; } = new List<ShipWeapon>();
    }
}
