namespace DMAdvantage.Shared.Entities
{
    public class InitativeEquipmentQuantity
    {
        public int Quantity { get; set; }

        public Guid InitativeDataId { get; set; }
        public InitativeData InitativeData { get; set; }
        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
    }
}
