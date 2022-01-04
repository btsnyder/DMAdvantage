using DMAdvantage.Shared.Models;
using System.Text.Json;

namespace DMAdvantage.Shared.Entities
{
    public class Encounter : BaseEntity
    {
        public string? Name { get; set; }
        public string? DataCache { get; set; }
        public List<InitativeData> Data => JsonSerializer.Deserialize<List<InitativeData>>(DataCache ?? string.Empty) ?? new();

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
