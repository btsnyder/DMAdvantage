namespace DMAdvantage.Shared.Entities
{
    public class TechPower : Power
    {
        public string Overcharge { get; set; }

        public override string OrderBy()
        {
            return $"{Level} - {Name}";
        }
    }
}
