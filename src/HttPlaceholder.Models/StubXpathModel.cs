using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubXpathModel
   {
      [YamlMember(Alias = "queryString")]
      public string QueryString { get; set; }

      [YamlMember(Alias = "namespaces")]
      public IDictionary<string, string> Namespaces { get; set; }
   }
}