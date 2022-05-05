namespace DMAdvantage.Shared.Entities
{
    public class ShipInitativeData
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public int Initative { get; set; }
        public int CurrentHull { get; set; }
        public int CurrentShield { get; set; }
        public int CurrentHullHitDice { get; set; }
        public int CurrentShieldHitDice { get; set; }
        public Guid? PlayerShipId { get; set; }
        public PlayerShip PlayerShip { get; set; }
        public Guid? EnemyShipId { get; set; }
        public EnemyShip EnemyShip { get; set; }
        public Ship Ship => PlayerShip == null ? EnemyShip : PlayerShip; 
        public bool IsPlayer => PlayerShip != null;

        public Guid ShipEncounterId { get; set; }
        public ShipEncounter Encounter { get; set; }

        public override bool Equals(object o)
        {
            var other = o as ShipInitativeData;
            return other?.Id == Id &&
                other?.User?.Id == User?.Id &&
                other?.Initative == Initative &&
                other?.CurrentHull == CurrentHull &&
                other?.CurrentShield == CurrentShield &&
                other?.CurrentHullHitDice == CurrentHullHitDice &&
                other?.CurrentShieldHitDice == CurrentShieldHitDice &&
                other?.Ship?.Id == Ship?.Id;
        }

        public override int GetHashCode() => $"{Id}:{Ship?.Id}".GetHashCode();

        public override string ToString()
        {
            return Ship?.Name;
        }
    }
}
