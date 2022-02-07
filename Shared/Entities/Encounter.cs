using DMAdvantage.Shared.Models;
using System.Text.Json;

namespace DMAdvantage.Shared.Entities
{
    public class Encounter : BaseEntity
    {
        public string? Name { get; set; }
        public Guid CurrentPlayer { get; set; }
        public string? DataCache { get; set; }
        public List<InitativeData> Data => JsonSerializer.Deserialize<List<InitativeData>>(DataCache ?? string.Empty) ?? new List<InitativeData>();
        public string? ConcentrationCache { get; set; }
        public Dictionary<string, Guid> ConcentrationPowers => JsonSerializer.Deserialize<Dictionary<string, Guid>>(ConcentrationCache ?? string.Empty) ?? new Dictionary<string, Guid>();

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
