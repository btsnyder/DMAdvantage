namespace DMAdvantage.Shared.Models
{
    public class InitativeData
    {
        public int Initative { get; set; }
        public Guid BeingId { get; set; }
        public int CurrentHP { get; set; }
        public int CurrentFP { get; set; }
        public int CurrentTP { get; set; }
        public bool Healing { get; set; }
        public bool Damaging { get; set; }
    }
}
