using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
    public class StubModel
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        [YamlMember(Alias = "conditions")]
        public StubConditionsModel Conditions { get; set; }

        [YamlMember(Alias = "negativeConditions")]
        public StubConditionsModel NegativeConditions { get; set; }

        [YamlMember(Alias = "response")]
        public StubResponseModel Response { get; set; }

        [YamlMember(Alias = "priority")]
        public int Priority { get; set; } = 0;

        [YamlMember(Alias = "tenant")]
        public string Tenant { get; set; }

        public StubMetadataModel Metadata { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}