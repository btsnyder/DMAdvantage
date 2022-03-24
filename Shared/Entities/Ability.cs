using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public class Ability : BaseEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }

        public new string Display => Name;

        public override bool Equals(object o)
        {
            var other = o as Ability;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
