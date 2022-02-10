namespace DMAdvantage.Shared.Models
{
    public class EncounterRequest : IEntityRequest
    {
        public string? Name { get; set; }
        public Guid CurrentPlayer { get; set; }
        public List<InitativeData> Data { get; set; } = new();
        public Dictionary<string, Guid> ConcentrationPowers { get; set; } = new();
    }
}
