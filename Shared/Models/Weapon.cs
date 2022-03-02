using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public WeaponType Type { get; set; }
        public bool Melee { get; set; }
        public string Description { get; set; }
        public string Damage { get; set; }
        public DamageType DamageType { get; set; }

        private IEnumerable<string> _properties = Array.Empty<string>();
        public IEnumerable<string> Properties
        {
            get => _properties;
            set
            {
                _properties = value;

                foreach (var prop in value)
                {
                    var description = PropertyDescriptions.FirstOrDefault(x => x.Name == prop);
                    if (description == null)
                    {
                        PropertyDescriptions.Add(new WeaponDescription { Name = prop });
                    }
                }

                var removed = PropertyDescriptions.Select(d => d.Name).Except(value);
                PropertyDescriptions.RemoveAll(x => removed.Contains(x.Name));
            }
        } 
        public List<WeaponDescription> PropertyDescriptions { get; set;} = new();
    }

    public class WeaponDescription
    { 
        public string Name { get; set; }
        public string Description { get; set; }

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
