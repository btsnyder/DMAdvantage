namespace DMAdvantage.Shared.Models
{
    public class InitativeDataModel : InitativeData
    {
        public IBeingResponse Being;

        public InitativeDataModel(IBeingResponse being)
        {
            Being = being;
            BeingId = being.Id;
            CurrentHP = being.HitPoints;
            CurrentFP = being.ForcePoints;
            CurrentTP = being.TechPoints;
            CurrentHitDice = being is CharacterResponse character ? character.Level : 0;

        }

        public InitativeDataModel(IBeingResponse being, InitativeData data)
        {
            Initative = data.Initative;
            BeingId = data.BeingId;
            CurrentHP = data.CurrentHP;
            CurrentFP = data.CurrentFP;
            CurrentTP = data.CurrentTP;
            CurrentHitDice = data.CurrentHitDice;
            Being = being;
        }

        public string Name => Being?.Name;
        public string ArmorClass => Being is CharacterResponse character ? character.ArmorClass.ToString() : string.Empty;
        public string ForcePoints => Being is CharacterResponse ? CurrentFP.ToString() : string.Empty;
        public string TechPoints => Being is CharacterResponse ? CurrentTP.ToString() : string.Empty;

        public void ApplyHP(int value)
        {
            CurrentHP += value;
        }

        public double HPAsDouble
        {
            get => CurrentHP;
            set => CurrentHP = (int)value;
        }

        public string GetHPDisplay()
        {
            switch (Being)
            {
                case null:
                    return string.Empty;
                case CharacterResponse:
                    return CurrentHP.ToString();
            }

            if (CurrentHP > Being.HitPoints * 0.75)
                return "Healthy";
            if (CurrentHP > Being.HitPoints * 0.5)
                return "Wounded";
            return CurrentHP > Being.HitPoints * 0.1 ? "Bloodied" : "Critical";
        }

        public string GetHPColor()
        {
            if (Being == null)
                return "black"; 
            if (CurrentHP > Being.HitPoints * 0.75)
                return "#00E676";
            if (CurrentHP > Being.HitPoints * 0.5)
                return "#EC9A0B";
            return CurrentHP > Being.HitPoints * 0.1 ? "#F06292" : "#E53935";
        }
    }
}
