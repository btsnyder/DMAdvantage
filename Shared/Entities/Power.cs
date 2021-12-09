using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public abstract class Power : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Level { get; set; }
        public CastingPeriod CastingPeriod { get; set; }
        public string? CastingDescription { get; set; }
        public PowerRange Range { get; set; }
        public string? RangeDescription { get; set; }
        public string? Duration { get; set; }
        public bool Concentration { get; set; }
        public HitOption? HitOption { get; set; }
        public string? HitDescription { get; set; }
    }
}
