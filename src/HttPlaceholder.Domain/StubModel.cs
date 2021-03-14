using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing all information about a stub.
    /// </summary>
    public class StubModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [YamlMember(Alias = "id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        [YamlMember(Alias = "conditions")]
        [JsonProperty("conditions")]
        public StubConditionsModel Conditions { get; set; } = new StubConditionsModel();

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [YamlMember(Alias = "response")]
        [JsonProperty("response")]
        public StubResponseModel Response { get; set; } = new StubResponseModel();

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        [YamlMember(Alias = "priority")]
        [JsonProperty("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [YamlMember(Alias = "tenant")]
        [JsonProperty("tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [YamlMember(Alias = "description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether this stub is enabled or not.
        /// </summary>
        [YamlMember(Alias = "enabled")]
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;
    }
}
