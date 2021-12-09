namespace DMAdvantage.Shared.Models
{
    public class EncounterRequest
    {
        public List<Guid> CharacterIds { get; set; } = new();
        public List<Guid> CreatureIds { get; set; } = new();
    }
}
