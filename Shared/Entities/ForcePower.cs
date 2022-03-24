using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class ForcePower : Power
    {
        public ForceAlignment Alignment { get; set; }
        public string Potency { get; set; }
        public Guid? PrerequisiteId { get; set; } 

        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public ICollection<Creature> Creatures { get; set; } = new List<Creature>();

        public override string OrderBy()
        {
            return $"{Level} - {Name}";
        }
        public new string Display => Name;
        public override bool Equals(object o)
        {
            var other = o as ForcePower;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
