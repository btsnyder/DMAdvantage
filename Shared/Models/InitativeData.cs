namespace DMAdvantage.Shared.Models
{
    public class InitativeData
    {
        public int Initative { get; set; }
        public Guid BeingId { get; set; }
        public int CurrentHP { get; set; }
        public bool Healing { get; set; }
        public bool Damaging { get; set; }
        public bool IsCharacter { get; set; }
    }
}
