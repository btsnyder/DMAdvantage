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
            if (being is CharacterResponse)
                IsCharacter = true;
        }

        public InitativeDataModel(IBeingResponse being, InitativeData data)
        {
            Initative = data.Initative;
            BeingId = data.BeingId;
            CurrentHP = data.CurrentHP;
            Healing = data.Healing;
            Damaging = data.Damaging;
            IsCharacter = data.IsCharacter;
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
