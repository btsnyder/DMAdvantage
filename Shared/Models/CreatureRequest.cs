using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Models
{
    public class CreatureRequest : BeingModel
    {
        public decimal ChallengeRating { get; set; }
        public List<BaseAction> Actions { get; set; } = new();
        public DamageType[] Vulnerabilities { get; set; } = Array.Empty<DamageType>();
        public DamageType[] Immunities { get; set; } = Array.Empty<DamageType>();
        public DamageType[] Resistances { get; set; } = Array.Empty<DamageType>();
    }
}
