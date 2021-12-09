using Microsoft.Extensions.Logging;

namespace TestEngineering.Mocks
{
    public class MockLog
    {
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public object? State { get; set; }
        public Exception? Exception { get; set; }
        public string Message { get; set; }

        public MockLog(LogLevel logLevel, EventId eventId, string message)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Message = message;
        }
    }
}
