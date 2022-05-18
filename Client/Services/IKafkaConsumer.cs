namespace DMAdvantage.Client.Services
{
    public interface IKafkaConsumer
    {
        Task ConnectAsync(string topic);
        event EventHandler<string?> OnMessageReceived;
    }
}
