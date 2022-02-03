namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to get a <see cref="IRequestLogger"/> instance for the current request.
/// </summary>
public interface IRequestLoggerFactory
{
    /// <summary>
    /// Get a <see cref="IRequestLogger"/> instance for the current request.
    /// </summary>
    /// <returns>The <see cref="IRequestLogger"/>.</returns>
    IRequestLogger GetRequestLogger();
}
