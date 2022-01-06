namespace DMAdvantage.Shared.Models
{
    public class EncounterRequest
    {
        public string? Name { get; set; }
        public Guid CurrentPlayer { get; set; }
        public List<InitativeData> Data { get; set; } = new();
    }
}
