using System.ComponentModel.DataAnnotations.Schema;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class Creature : Being
    {
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ChallengeRating { get; set; }
        public List<DamageType> Vulnerabilities { get; set; } = new();
        public List<DamageType> Immunities { get; set; } = new();
        public List<DamageType> Resistances { get; set; } = new();

        public ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();
        public ICollection<BaseAction> Actions { get; set; } = new List<BaseAction>();
    }
}
