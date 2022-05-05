namespace DMAdvantage.Shared.Entities
{
    public class ShipEncounter : BaseEntity
    {
        public Guid CurrentPlayer { get; set; }
        public ICollection<ShipInitativeData> InitativeData { get; set; } = new List<ShipInitativeData>();
    }
}
