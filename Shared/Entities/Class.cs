using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public class DMClass : BaseEntity, INamedEntity
    {
        [Required]
        public string Name { get; set; }
        public int HitDice { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
