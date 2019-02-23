using System;
using System.Collections.Generic;
using System.Reflection;

namespace HttPlaceholder.Utilities
{
    public static class ReflectionUtilities
    {
        // Source: https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
        public static IList<FieldInfo> GetConstants(Type type)
        {
            var constants = new List<FieldInfo>();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo fi in fieldInfos)
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
