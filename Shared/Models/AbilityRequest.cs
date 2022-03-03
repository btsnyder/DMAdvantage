using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Models
{
    public class AbilityRequest : IEntityRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
