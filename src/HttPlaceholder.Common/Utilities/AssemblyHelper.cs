using System.IO;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class that contains several assembly related methods.
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    ///     Returns the root path of the assembly the application is started as.
    /// </summary>
    /// <returns>The entry assembly root path.</returns>
    public static string GetEntryAssemblyRootPath()
    {
        var assembly = Assembly.GetEntryAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    ///     Returns the root path of the current assembly.
    /// </summary>
    /// <returns>The executing assembly root path.</returns>
    public static string GetExecutingAssemblyRootPath()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    ///     Returns the root path of the assembly that is calling this assembly.
    /// </summary>
    /// <returns>The root path of the calling assembly.</returns>
    public static string GetCallingAssemblyRootPath()
    {
        var assembly = Assembly.GetCallingAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    ///     Returns the assembly version of the entry assembly.
    /// </summary>
    /// <returns>The version as string.</returns>
    public static string GetAssemblyVersion() => Assembly.GetEntryAssembly().GetName().Version.ToString();
}
