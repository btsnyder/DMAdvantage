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
        public int? ArmorClass => Being?.ArmorClass;
        public string? Player => Being is CharacterResponse character ? character.PlayerName : string.Empty;

        public void ApplyHP(int value)
        {
            CurrentHP += value;
        }
    }
}
