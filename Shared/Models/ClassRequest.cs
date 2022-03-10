using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Models
{
    public class DMClassRequest : IEntityRequest
    {
        [Required]
        public string Name { get; set; }
        public int HitDice { get; set; } = 6;
    }
}
