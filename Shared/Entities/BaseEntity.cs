namespace DMAdvantage.Shared.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public abstract string OrderBy();
    }
}
