using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Models
{
    public class CharacterRequest : BeingModel
    {
        public string? PlayerName { get; set; }
        [Required]
        public int Level { get; set; }
    }
}
