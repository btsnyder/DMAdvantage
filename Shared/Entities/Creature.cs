using DMAdvantage.Shared.Enums;
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
        public List<BaseAction> Actions => JsonSerializer.Deserialize<List<BaseAction>>(ActionsCache ?? string.Empty) ?? new();
        public List<Guid> VulnerabilityIds { get; set; } = new();
        public List<Guid> ImmunityIds { get; set; } = new();
        public List<Guid> ResistanceIds { get; set; } = new();
        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
