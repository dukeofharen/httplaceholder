using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class that contains several assembly related methods.
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    /// Queries all implementations of a specific interface.
    /// </summary>
    /// <param name="assemblyFilter">If filled in, only return implementations where the namespace contains this string.</param>
    /// <typeparam name="TInterface">The interface the class should implement.</typeparam>
    /// <returns>A list of class types which implement the provided interface.</returns>
    public static IEnumerable<Type> GetImplementations<TInterface>(string assemblyFilter = "")
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies();
        if (!string.IsNullOrWhiteSpace(assemblyFilter))
        {
            assemblies = assemblies.Where(a => a.FullName.Contains(assemblyFilter)).ToArray();
        }

        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(TInterface).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
            .ToArray();

        return types;
    }

    /// <summary>
    /// Returns the root path of the assembly the application is started as.
    /// </summary>
    /// <returns>The entry assembly root path.</returns>
    public static string GetEntryAssemblyRootPath()
    {
        var assembly = Assembly.GetEntryAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Returns the root path of the current assembly.
    /// </summary>
    /// <returns>The executing assembly root path.</returns>
    public static string GetExecutingAssemblyRootPath()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Returns the root path of the assembly that is calling this assembly.
    /// </summary>
    /// <returns>The root path of the calling assembly.</returns>
    public static string GetCallingAssemblyRootPath()
    {
        var assembly = Assembly.GetCallingAssembly();
        var path = assembly.Location;
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Returns the assembly version of the entry assembly.
    /// </summary>
    /// <returns>The version as string.</returns>
    public static string GetAssemblyVersion() => Assembly.GetEntryAssembly().GetName().Version.ToString();
}
