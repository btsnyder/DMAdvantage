using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public class DMClass : BaseEntity
    {
        [Required]
        public int HitDice { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
