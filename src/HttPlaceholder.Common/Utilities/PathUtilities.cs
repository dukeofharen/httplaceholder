using System;
using System.IO;
using System.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// Contains several helper methods for working with file paths.
/// </summary>
public static class PathUtilities
{
    private static readonly string[] _unsafeStrings = {"$", "..", "?"};

    /// <summary>
    /// A method for removing unsafe characters from a path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string CleanPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var parts = path.Split(new[] {"/", "\\"}, StringSplitOptions.None);
        parts = parts.Where(p => !_unsafeStrings.Any(p.Contains)).ToArray();
        return string.Join(Path.DirectorySeparatorChar, parts);
    }
}
