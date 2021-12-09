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
        public string? VulnerabilitiesCache { get; set; }
        public DamageType[] Vulnerabilities => 
           JsonSerializer.Deserialize<DamageType[]>(VulnerabilitiesCache ?? string.Empty) ?? Array.Empty<DamageType>();
        public string? ImmunitiesCache { get; set; }
        public DamageType[] Immunities =>
           JsonSerializer.Deserialize<DamageType[]>(ImmunitiesCache ?? string.Empty) ?? Array.Empty<DamageType>();
        public string? ResistancesCahce { get; set; }
        public DamageType[] Resistances =>
           JsonSerializer.Deserialize<DamageType[]>(ResistancesCahce ?? string.Empty) ?? Array.Empty<DamageType>();
        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
