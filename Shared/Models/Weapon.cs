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
        public DamageType DamageType { get; set; }
        public IEnumerable<string> Properties { get; set; } = Array.Empty<string>();
        public List<WeaponDescription> PropertyDescriptions { get; set;} = new();
    }

    public class WeaponDescription
    { 
        public string? Name { get; set; }
        public string? Description { get; set; }

        public override string ToString()
        {
            var name = Name;
            if (!string.IsNullOrWhiteSpace(Description))
            {
                name += $" ({Description})";
            }
            return name ?? string.Empty;
        }
    }
}
