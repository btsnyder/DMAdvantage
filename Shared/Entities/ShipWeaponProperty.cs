namespace DMAdvantage.Shared.Entities
{
    public class ShipWeaponProperty : BaseEntity
    {
        public string Description { get; set; }
        public string Modifier { get; set; }

        public ICollection<ShipWeapon> Weapons { get; set; } = new List<ShipWeapon>();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Modifier))
                return Name;
            return $"{Name} ({Modifier})";
        }
    }
}
