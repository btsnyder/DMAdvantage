namespace DMAdvantage.Shared.Entities
{
    public class Ability : BaseEntity
    {
        public string Description { get; set; }

        public ICollection<Character> Characters { get; set; }

        public override string ToString() => Name;
    }
}
