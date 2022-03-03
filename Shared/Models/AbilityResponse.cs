namespace DMAdvantage.Shared.Models
{
    public class AbilityResponse : AbilityRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => Name;
    }
}
