using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Models
{
    public class ShipInitativeDataModel : ShipInitativeData
    {
        public ShipInitativeDataModel(Ship ship)
        {
            Id = Guid.NewGuid();
            if (ship is PlayerShip playerShip)
            {
                PlayerShip = playerShip;
                PlayerShipId = playerShip.Id;
            }
            else if (ship is EnemyShip enemyShip)
            {
                EnemyShip = enemyShip;
                EnemyShipId = enemyShip.Id;
            }
            CurrentHull = ship.HullPoints;
            CurrentHullHitDice = ship.HullHitDiceNumber;
            CurrentShield = ship.ShieldPoints;
            CurrentShieldHitDice = ship.ShieldHitDiceNumber;
        }

        public ShipInitativeDataModel(Ship ship, ShipInitativeData data)
        {
            Id = Guid.NewGuid();
            Initative = data.Initative;
            if (ship is PlayerShip playerShip)
            {
                PlayerShip = playerShip;
                PlayerShipId = playerShip.Id;
            }
            else if (ship is EnemyShip enemyShip)
            {
                EnemyShip = enemyShip;
                EnemyShipId = enemyShip.Id;
            }
            CurrentHull = data.CurrentHull;
            CurrentHullHitDice = data.CurrentHullHitDice;
            CurrentShield = data.CurrentShield;
            CurrentShieldHitDice = data.CurrentShieldHitDice;
        }

        public string Name => Ship?.Name;
        public string ArmorClass => IsPlayer ? PlayerShip.ArmorClass.ToString() : string.Empty;

        public void ApplyHP(int value)
        {
            CurrentHull += value;
        }

        public void ApplySP(int value)
        {
            CurrentShield += value;
        }

        public double HPAsDouble
        {
            get => CurrentHull;
            set => CurrentHull = (int)value;
        }

        public double SPAsDouble
        {
            get => CurrentShield;
            set => CurrentShield = (int)value;
        }

        public string GetHPDisplay()
        {
            if (Ship == null)
                return string.Empty;
            if (IsPlayer)
                return CurrentHull.ToString();

            if (CurrentHull > Ship.HullPoints * 0.75)
                return "Healthy";
            if (CurrentHull > Ship.HullPoints * 0.5)
                return "Wounded";
            return CurrentHull > Ship.HullPoints * 0.1 ? "Bloodied" : "Critical";
        }

        public string GetHPColor()
        {
            if (Ship == null)
                return "black"; 
            if (CurrentHull > Ship.HullPoints * 0.75)
                return "#00E676";
            if (CurrentHull > Ship.HullPoints* 0.5)
                return "#EC9A0B";
            return CurrentHull > Ship.HullPoints * 0.1 ? "#F06292" : "#E53935";
        }

        public string GetSPDisplay()
        {
            if (Ship == null)
                return string.Empty;
            if (IsPlayer)
                return CurrentShield.ToString();

            if (CurrentShield > Ship.ShieldPoints * 0.75)
                return "Healthy";
            if (CurrentShield > Ship.ShieldPoints * 0.5)
                return "Wounded";
            return CurrentShield > Ship.ShieldPoints * 0.1 ? "Bloodied" : "Critical";
        }

        public string GetSPColor()
        {
            if (Ship == null)
                return "black";
            if (CurrentShield > Ship.ShieldPoints * 0.75)
                return "#00E676";
            if (CurrentShield > Ship.ShieldPoints * 0.5)
                return "#EC9A0B";
            return CurrentShield > Ship.ShieldPoints * 0.1 ? "#F06292" : "#E53935";
        }
    }
}
