using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class ForcePowerRequest : PowerModel
    {
        public ForceAlignment Alignment { get; set; }
        public string? Potency { get; set; }
        public Guid? PrerequisiteId { get; set; }
    }
}
