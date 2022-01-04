namespace DMAdvantage.Shared.Models
{
    public class InitativeDataModel : InitativeData
    {
        public IBeingResponse? Being;

        public InitativeDataModel(IBeingResponse being)
        {
            Being = being;
            BeingId = being.Id;
            CurrentHP = being.HitPoints;
            CurrentFP = being.ForcePoints;
            CurrentTP = being.TechPoints;
        }

        public InitativeDataModel(IBeingResponse being, InitativeData data)
        {
            Initative = data.Initative;
            BeingId = data.BeingId;
            CurrentHP = data.CurrentHP;
            CurrentFP = data.CurrentFP;
            CurrentTP = data.CurrentTP;
            Healing = data.Healing;
            Damaging = data.Damaging;
            Being = being;
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
