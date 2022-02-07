using DMAdvantage.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DMAdvantage.Shared.Entities
{
    public class Creature : Being
    {
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ChallengeRating { get; set; }
        public string? ActionsCache { get; set; }
        public List<BaseAction> Actions => JsonSerializer.Deserialize<List<BaseAction>>(ActionsCache ?? string.Empty) ?? new List<BaseAction>();
        public List<string> Vulnerabilities { get; set; } = new();
        public List<string> Immunities { get; set; } = new();
        public List<string> Resistances { get; set; } = new();
        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
