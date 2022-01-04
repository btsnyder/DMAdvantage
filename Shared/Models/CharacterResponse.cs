namespace DMAdvantage.Shared.Models
{
    public class CharacterResponse : CharacterRequest, IBeingResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;
    }
}
