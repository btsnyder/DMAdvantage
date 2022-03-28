using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class Weapon : BaseEntity
    {
        public WeaponType Type { get; set; }
        public bool Melee { get; set; }
        public string Description { get; set; }
        public string Damage { get; set; }
        public DamageType DamageType { get; set; }
        public string PropertyDescription { get; set; }

        public ICollection<WeaponProperty> Properties { get; set; } = new List<WeaponProperty>();
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
