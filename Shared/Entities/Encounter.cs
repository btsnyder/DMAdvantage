using System.Text.Json;

namespace DMAdvantage.Shared.Entities
{
    public class Encounter : BaseEntity
    {
        public Guid CurrentPlayer { get; set; }
        public ICollection<InitativeData> InitativeData { get; set; } = new List<InitativeData>();

        public string ConcentrationCache { get; set; }
        public Dictionary<string, Guid> ConcentrationPowers => JsonSerializer.Deserialize<Dictionary<string, Guid>>(ConcentrationCache ?? string.Empty) ?? new Dictionary<string, Guid>();
    }
}
