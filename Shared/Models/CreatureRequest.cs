using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class CreatureRequest : BeingModel, IEntityRequest
    {
        public decimal ChallengeRating { get; set; }
        public List<BaseAction> Actions { get; set; } = new();
        public IEnumerable<DamageType> Vulnerabilities { get; set; } = Array.Empty<DamageType>();
        public IEnumerable<string> Immunities { get; set; } = Array.Empty<string>();
        public IEnumerable<string> Resistances { get; set; } = Array.Empty<string>();
    }
}
