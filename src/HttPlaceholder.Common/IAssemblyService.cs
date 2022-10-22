namespace HttPlaceholder.Common;

/// <summary>
///     Describes a class that is used for working with assemblies.
/// </summary>
public interface IAssemblyService
{
    /// <summary>
    ///     Returns the root path of the assembly the application is started as.
    /// </summary>
    /// <returns>The entry assembly root path.</returns>
    string GetEntryAssemblyRootPath();

    /// <summary>
    ///     Returns the root path of the current assembly.
    /// </summary>
    /// <returns>The executing assembly root path.</returns>
    string GetExecutingAssemblyRootPath();

    /// <summary>
    ///     Returns the assembly version of the entry assembly.
    /// </summary>
    /// <returns>The version as string.</returns>
    string GetAssemblyVersion();
}
