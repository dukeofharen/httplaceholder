using System;

namespace HttPlaceholder.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigKeyAttribute : Attribute
    {
        public string Description { get; set; }

        public string Example { get; set; }
    }
}
