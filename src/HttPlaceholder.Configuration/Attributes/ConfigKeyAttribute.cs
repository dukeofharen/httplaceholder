using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HttPlaceholder.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigKeyAttribute : Attribute
    {
        public string Description { get; set; }

        public string Example { get; set; }

        public string ConfigPath { get; set; }

        public bool IsBoolValue { get; set; } = false;
    }
}
