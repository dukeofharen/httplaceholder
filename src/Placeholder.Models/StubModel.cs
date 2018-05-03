using YamlDotNet.Serialization;

namespace Placeholder.Models
{
   public class StubModel
   {
      [YamlMember(Alias = "id")]
      public string Id { get; set; }

      [YamlMember(Alias = "conditions")]
      public StubConditionsModel Conditions { get; set; }

      [YamlMember(Alias = "response")]
      public StubResponseModel Response { get; set; }
   }
}
