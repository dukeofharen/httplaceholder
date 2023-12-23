namespace HttPlaceholder.Common.FileWatchers;

/// <summary>
///     Describes a class that is used to get an instance of <see cref="IFileWatcherBuilder"/>.
/// </summary>
public interface IFileWatcherBuilderFactory
{
    /// <summary>
    ///     Creates an instance of <see cref="IFileWatcherBuilder"/>.
    /// </summary>
    /// <returns>An instance of <see cref="IFileWatcherBuilder"/>.</returns>
    IFileWatcherBuilder CreateBuilder();
}
