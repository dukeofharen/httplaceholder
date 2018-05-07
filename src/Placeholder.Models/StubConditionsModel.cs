using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Placeholder.Models
{
   public class StubConditionsModel
   {
      [YamlMember(Alias = "method")]
      public string Method { get; set; }

      [YamlMember(Alias = "url")]
      public StubUrlConditionModel Url { get; set; }

      [YamlMember(Alias = "body")]
      public IEnumerable<string> Body { get; set; }

      [YamlMember(Alias = "headers")]
      public IDictionary<string, string> Headers { get; set; }

      [YamlMember(Alias = "xpath")]
      public IEnumerable<StubXpathModel> Xpath { get; set; }

      [YamlMember(Alias = "jsonPath")]
      public IEnumerable<string> JsonPath { get; set; }
   }
}
