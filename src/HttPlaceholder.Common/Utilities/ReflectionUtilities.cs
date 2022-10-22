using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with reflection.
/// </summary>
public static class ReflectionUtilities
{
    // Source: https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
    /// <summary>
    ///     Returns a list of constants from a given type.
    /// </summary>
    /// <param name="type">The type to look up the constants for.</param>
    /// <returns>A list of <see cref="FieldInfo" />.</returns>
    public static IEnumerable<FieldInfo> GetConstants(Type type) => type
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .ToList();
}
