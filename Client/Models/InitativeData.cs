using DMAdvantage.Shared.Models;

namespace DMAdvantage.Client.Models
{
    public class InitativeData
    {
        public int Initative { get; set; }
        public BeingModel Being { get; set; }
        public int CurrentHP { get; set; }
        public bool Healing { get; set; }
        public bool Damaging { get; set; }

        public InitativeData(BeingModel model)
        {
            Being = model;
            CurrentHP = model.HitPoints;
        }

        public void ApplyHP(int value)
        {
            if (Healing)
                CurrentHP += value;
            else
                CurrentHP -= value;
        }
    }
}
