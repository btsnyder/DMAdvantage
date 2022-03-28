namespace DMAdvantage.Shared.Entities
{
    public class WeaponProperty : BaseEntity
    {
        public string Description { get; set; }
        public string Modifier { get; set; }

        public ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Modifier))
                return Name;
            return $"{Name} ({Modifier})";
        }
    }
}
