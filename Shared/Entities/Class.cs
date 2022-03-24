using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public class DMClass : BaseEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }
        public int HitDice { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }

        public override bool Equals(object o)
        {
            var other = o as DMClass;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
