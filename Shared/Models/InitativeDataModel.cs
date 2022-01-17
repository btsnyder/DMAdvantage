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
            Being = being;
        }

        public string? Name => Being?.Name;
        public string? ArmorClass => Being is CharacterResponse character ? character.ArmorClass.ToString() : string.Empty;
        public string? Player => Being is CharacterResponse character ? character.PlayerName : string.Empty;

        public void ApplyHP(int value)
        {
            CurrentHP += value;
        }

        public double HPAsDouble
        {
            get => CurrentHP;
            set
            {
                CurrentHP = (int)value;
            }
        }

        public string GetHPDisplay()
        {
            if (Being == null)
                return string.Empty;
            if (Being is CharacterResponse)
            {
                return CurrentHP.ToString();
            }
            if (CurrentHP > Being.HitPoints * 0.75)
                return "Healthy";
            if (CurrentHP > Being.HitPoints * 0.5)
                return "Wounded";
            if (CurrentHP > Being.HitPoints * 0.1)
                return "Bloodied";
            return "Critical";
        }

        public string GetHPColor()
        {
            if (Being == null)
                return "black"; 
            if (CurrentHP > Being.HitPoints * 0.75)
                return "#208427";
            if (CurrentHP > Being.HitPoints * 0.5)
                return "#EC9A0B";
            if (CurrentHP > Being.HitPoints * 0.1)
                return "#FF0049";
            return "#820629";
        }
    }
}
