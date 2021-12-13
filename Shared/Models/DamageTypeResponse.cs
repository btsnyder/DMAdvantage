namespace DMAdvantage.Shared.Models
{
    public class DamageTypeResponse : DamageTypeRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;
    }
}
