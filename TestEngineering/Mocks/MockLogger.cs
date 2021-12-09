using Microsoft.Extensions.Logging;

namespace TestEngineering.Mocks
{
    public class MockLogger<T> : ILogger<T>, IDisposable
    {
        public Queue<MockLog> Logs { get; } = new Queue<MockLog>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            Logs.Enqueue(new MockLog(logLevel, eventId, exception == null ? string.Empty : formatter(state, exception))
            {
                Exception = exception,
                State = state
            });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
