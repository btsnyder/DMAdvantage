using DMAdvantage.Shared.Services.Kafka;
using Xunit;

namespace TestEngineering.Data
{
    public class KafkaTopicsTestData : TheoryData<string>
    {
        public KafkaTopicsTestData()
        {
            Add(Topics.Encounters);
            Add(Topics.ShipEncounters);
        }
    }
}
