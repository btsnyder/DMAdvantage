using Confluent.Kafka;

namespace DMAdvantage.Shared.Services.Kafka
{
    public class KafkaProducer
    {
        private readonly Dictionary<string, string> _config;
        private IProducer<string, string> _producer;
        private bool _connected;
        private DeliveryReport<string, string> _connectedval;

        public KafkaProducer()
        {
            var host = Environment.GetEnvironmentVariable("DM_KAFKA");
            _config = new Dictionary<string, string>
            {
                { "bootstrap.servers", host }
            };
        }

        public void Start()
        {
            var builder = new ProducerBuilder<string, string>(_config);
            _producer = builder.Build();
            _connected = true;
        }

        public void Stop()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }

        public void SendMessage(KafkaMessage message)
        {
            if (_producer == null || !_connected)
                return;
            _producer.Produce(message.Topic, new Message<string, string> { Key = message.User, Value = message.Value },
                (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        _connected = false;
                        _connectedval = deliveryReport;
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        _connectedval = deliveryReport;
                        Console.WriteLine($"Produced event to topic {message.Topic}: key = {message.User,-10} value = {message.Value}");
                    }
                });
        }
    }
}
