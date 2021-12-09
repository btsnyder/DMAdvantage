namespace DMAdvantage.Shared.Entities
{
    public class DamageType : BaseEntity
    {
        public string? Name { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
