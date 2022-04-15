using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Models
{
    public class InitativeDataModel : InitativeData
    {
        public InitativeDataModel(Being being)
        {
            Id = Guid.NewGuid();
            if (being is Character character)
            {
                Character = character;
                CharacterId = character.Id;
            }
            else if (being is Creature creature)
            {
                Creature = creature;
                CreatureId = creature.Id;
            }
            CurrentHP = being.HitPoints;
            CurrentFP = being.ForcePoints;
            CurrentTP = being.TechPoints;
            CurrentHitDice = IsCharacter ? Character.Level : 0;

        }

        public InitativeDataModel(Being being, InitativeData data)
        {
            Id = Guid.NewGuid();
            Initative = data.Initative;
            if (being is Character character)
            {
                Character = character;
                CharacterId= character.Id;
            }
            else if (being is Creature creature)
            {
                Creature = creature;
                CreatureId = creature.Id;
            }
            CurrentHP = data.CurrentHP;
            CurrentFP = data.CurrentFP;
            CurrentTP = data.CurrentTP;
            CurrentHitDice = data.CurrentHitDice;
        }

        public string Name => Being?.Name;
        public string ArmorClass => IsCharacter ? Character.ArmorClass.ToString() : string.Empty;
        public string ForcePoints => IsCharacter ? CurrentFP.ToString() : string.Empty;
        public string TechPoints => IsCharacter ? CurrentTP.ToString() : string.Empty;

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
            if (Character == null && Creature == null)
                return string.Empty;
            if (IsCharacter)
                return CurrentHP.ToString();

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
