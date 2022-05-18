using DMAdvantage.Shared.Services.Kafka;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DMAdvantage.IntegrationTests
{
    public class KafkaTests
    {
        private List<KafkaMessage> _messages = new();

        [Fact]
        public async Task SendMessage()
        {
            var producer = new KafkaProducer();
            var consumer = new KafkaConsumer();
            consumer.Start("quickstart-events");
            producer.Start();
            consumer.MessageReceived += AddMessage;
            producer.SendMessage(new KafkaMessage { Topic = "quickstart-events", User = "bob", Value = "this is my message 2" });
            await Task.Delay(1000);
            _messages.Should().HaveCount(1);
            producer.Stop();
            consumer.Stop();
        }

        private void AddMessage(object? sender, KafkaMessage message)
        {
            _messages.Add(message);
        }
    }
}
