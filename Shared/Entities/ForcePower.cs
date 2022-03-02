using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class ForcePower : Power
    {
        public ForceAlignment Alignment { get; set; }
        public string Potency { get; set; }
        public Guid? PrerequisiteId { get; set; } 

        public override string OrderBy()
        {
            return $"{Level} - {Name}";
        }
    }
}
