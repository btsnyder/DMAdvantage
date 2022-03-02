namespace DMAdvantage.Shared.Entities
{
    public class Ability : BaseEntity, INamedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
