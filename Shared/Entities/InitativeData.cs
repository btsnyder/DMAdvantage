namespace DMAdvantage.Shared.Entities
{
    public class InitativeData
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public int Initative { get; set; }
        public int CurrentHP { get; set; }
        public int CurrentFP { get; set; }
        public int CurrentTP { get; set; }
        public int CurrentHitDice { get; set; }
        public Guid? CharacterId { get; set; }
        public Character Character { get; set; }
        public Guid? CreatureId { get; set; }
        public Creature Creature { get; set; }
        public Being Being => Character == null ? Creature : Character;
        public bool IsCharacter => Character != null;

        public Guid EncounterId { get; set; }
        public Encounter Encounter { get; set; }

        public override bool Equals(object o)
        {
            var other = o as InitativeData;
            return other?.Id == Id &&
                other?.User?.Id == User?.Id &&
                other?.Initative == Initative &&
                other?.CurrentHP == CurrentHP &&
                other?.CurrentFP == CurrentFP &&
                other?.CurrentTP == CurrentTP &&
                other?.CurrentHitDice == CurrentHitDice &&
                other?.Being?.Id == Being?.Id;
        }

        public override int GetHashCode() => $"{Id}:{Being?.Id}".GetHashCode();

        public override string ToString()
        {
            return Character == null ? Creature?.Name : Character.Name;
        }
    }
}
