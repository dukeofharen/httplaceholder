using System;
using System.Collections.Generic;
using System.Reflection;

namespace HttPlaceholder.Common.Utilities
{
    public static class ReflectionUtilities
    {
        // Source: https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
        public static IList<FieldInfo> GetConstants(Type type)
        {
            var constants = new List<FieldInfo>();
            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    constants.Add(fi);
                }
            }

            return constants;
        }
    }
}
