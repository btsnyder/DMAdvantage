namespace DMAdvantage.Shared.Models
{
    public class TechPowerResponse : TechPowerRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => $"{Level} - {Name}";
    }
}
