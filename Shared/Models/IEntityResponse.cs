namespace DMAdvantage.Shared.Models
{
    public interface IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display { get; }
    }
}
