namespace DMAdvantage.Shared.Models
{
    public class DMClassResponse : DMClassRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => Name;

        public override bool Equals(object o)
        {
            var other = o as DMClassResponse;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
