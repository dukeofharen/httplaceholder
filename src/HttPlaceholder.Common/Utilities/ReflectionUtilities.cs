using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities;

public static class ReflectionUtilities
{
    // Source: https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
    public static IEnumerable<FieldInfo> GetConstants(Type type) => type
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .ToList();
}