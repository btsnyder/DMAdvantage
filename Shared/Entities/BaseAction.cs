using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class BaseAction : BaseEntity
    {
        public string Description { get; set; }
        public string Hit { get; set; } 
        public string Range { get; set; } 
        public string Damage { get; set; }
        public DamageType DamageType { get; set; }

        public ICollection<Creature> Creatures { get; set; }
    }
}
