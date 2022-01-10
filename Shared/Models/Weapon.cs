using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class Weapon
    {
        public string? Name { get; set; }
        public WeaponType Type { get; set; }
        public bool Melee { get; set; }
        public string? Description { get; set; }
        public string? Damage { get; set; }
        public IEnumerable<string> Properties { get; set; } = Array.Empty<string>();
    }
}
