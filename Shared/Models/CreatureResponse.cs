namespace DMAdvantage.Shared.Models
{
    public class CreatureResponse : CreatureRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;
    }
}
