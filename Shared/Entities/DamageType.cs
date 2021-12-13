using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public class DamageType : BaseEntity
    {
        [Required]
        public string? Name { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
