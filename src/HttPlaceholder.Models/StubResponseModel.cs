using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubResponseModel
   {
      [YamlMember(Alias = "statusCode")]
      public int? StatusCode { get; set; }

      [YamlMember(Alias = "text")]
      public string Text { get; set; }

      [YamlMember(Alias = "base64")]
      public string Base64 { get; set; }

      [YamlMember(Alias = "file")]
      public string File { get; set; }

      [YamlMember(Alias = "headers")]
      public IDictionary<string, string> Headers { get; set; }

      [YamlMember(Alias = "extraDuration")]
      public int? ExtraDuration { get; set; }

      [YamlMember(Alias = "json")]
      public string Json { get; set; }
   }
}
