using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.Stubs
{
    /// <summary>
    /// A model for storing all information about a stub.
    /// </summary>
    public class StubDto : IMapFrom<StubModel>, IMapTo<StubModel>
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
        public StubConditionsDto Conditions { get; set; }

        /// <summary>
        /// Gets or sets the negative conditions.
        /// </summary>
        [YamlMember(Alias = "negativeConditions")]
        public StubConditionsDto NegativeConditions { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [YamlMember(Alias = "response")]
        public StubResponseDto Response { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        [YamlMember(Alias = "priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [YamlMember(Alias = "tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
    }
}
