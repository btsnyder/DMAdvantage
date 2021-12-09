using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class CreatureRequest : BeingModel
    {
        public decimal ChallengeRating { get; set; }
        public List<BaseAction> Actions { get; set; } = new();
        public List<Guid> VulnerabilityIds { get; set; } = new();
        public List<Guid> ImmunityIds { get; set; } = new();
        public List<Guid> ResistanceIds { get; set; } = new();
    }
}
