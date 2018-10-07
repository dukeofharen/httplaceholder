using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubUrlConditionModel
   {
      [YamlMember(Alias = "path")]
      public string Path { get; set; }

      [YamlMember(Alias = "query")]
      public IDictionary<string, string> Query { get; set; }

      [YamlMember(Alias = "fullPath")]
      public string FullPath { get; set; }

      [YamlMember(Alias = "isHttps")]
      public bool? IsHttps { get; set; }
   }
}