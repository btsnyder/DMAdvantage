using Confluent.Kafka;

namespace DMAdvantage.Shared.Services.Kafka
{
    public class KafkaConsumer
    {
        private readonly Dictionary<string, string> _config;
        private IConsumer<string, string> _consumer;
        private bool _listening;
        private CancellationTokenSource _tokenSource = new();

        public (Guid, KafkaMessage) LastConsumed { get; private set; } = (Guid.Empty, null);

        public KafkaConsumer()
        {
            var host = Environment.GetEnvironmentVariable("DM_KAFKA");
            _config = new Dictionary<string, string>
            {
                { "bootstrap.servers", host },
                { "group.id", "dm-advantage" }
            };
        }

        public void Start(string topic = null)
        {
            var builder = new ConsumerBuilder<string, string>(_config);
            _consumer = builder.Build();
            if (topic != null)
                Task.Run(() => Listen(topic));
        }

        public void Listen(string topic)
        {
            _listening = true;
            _tokenSource.Cancel();
            _tokenSource = new();
            _consumer.Subscribe(topic);
            while (_listening)
            {
                var cr = _consumer.Consume(_tokenSource.Token);
                var message = new KafkaMessage { Topic = cr.Topic, User = cr.Message.Key, Value = cr.Message.Value };
                LastConsumed = (Guid.NewGuid(), message);
                MessageReceived?.Invoke(this, message);
            }
        }

        public KafkaMessage Consume(string topic)
        {
            _consumer.Subscribe(topic);
            var cr = _consumer.Consume(_tokenSource.Token);
            var message = new KafkaMessage { Topic = cr.Topic, User = cr.Message.Key, Value = cr.Message.Value };
            LastConsumed = (Guid.NewGuid(), message);
            return message;
        }

        public event EventHandler<KafkaMessage> MessageReceived;

        public async void Stop()
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                _listening = false;
                await Task.Delay(100);
                _consumer.Close();
            }
        }
    }
}
