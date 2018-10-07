using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubFormModel
   {
      [YamlMember(Alias = "key")]
      public string Key { get; set; }

      [YamlMember(Alias = "value")]
      public string Value { get; set; }
   }
}
