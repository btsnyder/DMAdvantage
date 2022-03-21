namespace DMAdvantage.Shared.Models
{
    public class ForcePowerResponse : ForcePowerRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => $"{Level} - {Name}";

        public override bool Equals(object o)
        {
            var other = o as ForcePowerResponse;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
