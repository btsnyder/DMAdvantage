namespace DMAdvantage.Shared.Models
{
    public class CreatureRequest : BeingModel
    {
        public decimal ChallengeRating { get; set; }
        public List<BaseAction> Actions { get; set; } = new();
        public IEnumerable<string> Vulnerabilities { get; set; } = Array.Empty<string>();
        public IEnumerable<string> Immunities { get; set; } = Array.Empty<string>();
        public IEnumerable<string> Resistances { get; set; } = Array.Empty<string>();
    }
}
