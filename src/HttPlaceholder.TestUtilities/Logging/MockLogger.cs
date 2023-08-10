using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.TestUtilities.Logging;

public class MockLogger<TCategoryName> : ILogger<TCategoryName>
{
    public IList<LogEntry> Entries { get; } = new List<LogEntry>();

    public IDisposable BeginScope<TState>(TState state) =>
        new Mock<IDisposable>().Object;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var entry = new LogEntry
        {
            Exception = exception, State = state.ToString(), EventId = eventId, LogLevel = logLevel
        };
        Entries.Add(entry);
        Debug.WriteLine(entry);
    }

    public bool Contains(LogLevel logLevel, string text) => Entries.Any(e =>
        e.LogLevel == logLevel &&
        e.State.Contains(text));

    public bool ContainsWithExactText(LogLevel logLevel, string text) => Entries.Any(e =>
        e.LogLevel == logLevel &&
        e.State == text);

    public bool HasEntries() => Entries.Any();

    public class LogEntry
    {
        public LogLevel LogLevel { get; set; }

        public EventId EventId { get; set; }

        public string State { get; set; }

        public Exception Exception { get; set; }

        public override string ToString() => $"{LogLevel} {EventId} {State} {Exception}";
    }
}
