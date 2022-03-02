namespace DMAdvantage.Shared.Models
{
    public class CreatureResponse : CreatureRequest, IBeingResponse
    {
        public Guid Id { get; set; }
        public string Display => Name;
    }
}
