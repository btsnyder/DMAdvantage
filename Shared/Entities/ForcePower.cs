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
    }
}
