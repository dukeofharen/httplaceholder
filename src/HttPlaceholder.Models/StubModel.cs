using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
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
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        [YamlMember(Alias = "conditions")]
        public StubConditionsModel Conditions { get; set; }

        /// <summary>
        /// Gets or sets the negative conditions.
        /// </summary>
        [YamlMember(Alias = "negativeConditions")]
        public StubConditionsModel NegativeConditions { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [YamlMember(Alias = "response")]
        public StubResponseModel Response { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        [YamlMember(Alias = "priority")]
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [YamlMember(Alias = "tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }
    }
}