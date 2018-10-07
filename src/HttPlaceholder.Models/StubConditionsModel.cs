using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubConditionsModel
   {
      [YamlMember(Alias = "method")]
      public string Method { get; set; }

      [YamlMember(Alias = "url")]
      public StubUrlConditionModel Url { get; set; }

      [YamlMember(Alias = "body")]
      public IEnumerable<string> Body { get; set; }

      [YamlMember(Alias = "form")]
      public IEnumerable<StubFormModel> Form { get; set; }

      [YamlMember(Alias = "headers")]
      public IDictionary<string, string> Headers { get; set; }

      [YamlMember(Alias = "xpath")]
      public IEnumerable<StubXpathModel> Xpath { get; set; }

      [YamlMember(Alias = "jsonPath")]
      public IEnumerable<string> JsonPath { get; set; }

      [YamlMember(Alias = "basicAuthentication")]
      public StubBasicAuthenticationModel BasicAuthentication { get; set; }

      [YamlMember(Alias = "clientIp")]
      public string ClientIp { get; set; }

      [YamlMember(Alias = "host")]
      public string Host { get; set; }
   }
}