namespace DMAdvantage.Shared.Models
{
    public class CharacterResponse : CharacterRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => Name ?? string.Empty;
    }
}
