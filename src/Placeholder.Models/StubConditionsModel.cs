using YamlDotNet.Serialization;

namespace Placeholder.Models
{
   public class StubConditionsModel
   {
      [YamlMember(Alias = "method")]
      public string Method { get; set; }

      [YamlMember(Alias = "url")]
      public StubUrlConditionModel Url { get; set; }
   }
}
