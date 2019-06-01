using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Utilities
{
    public class MockLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string line = $"{logLevel} - {eventId} - {state} - {exception}";
            Debug.WriteLine(line);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Mock<IDisposable>().Object;
        }
    }
}
