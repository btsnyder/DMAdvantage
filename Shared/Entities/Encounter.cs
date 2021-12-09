namespace DMAdvantage.Shared.Entities
{
    public class Encounter : BaseEntity
    {
        public List<Guid> CharacterIds { get; set; } = new();
        public List<Guid> CreatureIds { get; set; } = new();

        public override string OrderBy()
        {
            return Id.ToString();
        }
    }
}
