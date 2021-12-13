
namespace DMAdvantage.Shared.Models
{
    public class ForcePowerResponse : ForcePowerRequest, IEntityResponse
    {
        public Guid Id { get; set; }
        public string Display => $"{Level} - {Name}";
    }
}
