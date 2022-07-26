using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class Equipment : BaseEntity
    {
        public EquipmentCategory Category { get; set; }
        public string Description { get; set; }

        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public List<InitativeEquipmentQuantity> InitativeData { get; set; }
    }
}
