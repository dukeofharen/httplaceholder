using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Placeholder.Models
{
    public class StubUrlConditionModel
    {
       [YamlMember(Alias = "path")]
       public string Path { get; set; }

       [YamlMember(Alias = "query")]
       public IDictionary<string, string> Query { get; set; }
    }
}
