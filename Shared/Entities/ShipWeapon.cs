using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class ShipWeapon : BaseEntity
    {
        public ShipWeaponType Type { get; set; }
        public string Description { get; set; }
        public string Damage { get; set; }
        public DamageType DamageType { get; set; }

        public ICollection<ShipWeaponProperty> Properties { get; set; } = new List<ShipWeaponProperty>();
        public ICollection<Ship> Ships { get; set; } = new List<Ship>();
    }
}
