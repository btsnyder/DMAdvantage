namespace DMAdvantage.Shared.Entities
{
    public class PlayerShip : Ship
    {
        public bool? StrengthSave { get; set; } = false;
        public bool? DexteritySave { get; set; } = false;
        public bool? ConstitutionSave { get; set; } = false;
        public bool? IntelligenceSave { get; set; } = false;
        public bool? WisdomSave { get; set; } = false;
        public bool? CharismaSave { get; set; } = false;
        public bool? Boost { get; set; } = false;
        public bool? Ram { get; set; } = false;
        public bool? Hide { get; set; } = false;
        public bool? Maneuvering { get; set; } = false;
        public bool? Patch { get; set; } = false;
        public bool? Regulation { get; set; } = false;
        public bool? Astrogation { get; set; } = false;
        public bool? Data { get; set; } = false;
        public bool? Probe { get; set; } = false;
        public bool? Scan { get; set; } = false;
        public bool? Impress { get; set; } = false;
        public bool? Interfere { get; set; } = false;
        public bool? Menace { get; set; } = false;
        public bool? Swindle { get; set; } = false;
    }
}
