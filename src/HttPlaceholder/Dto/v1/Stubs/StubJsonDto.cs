using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.v1.Stubs
{
    /// <summary>
    /// A model for storing the stub JSON condition model.
    /// </summary>
    public class StubJsonDto : IMapFrom<StubJsonModel>, IMapTo<StubJsonModel>
    {
        /// <summary>
        /// Gets or sets whether the posted JSON must be exactly in order as specified in the stub.
        /// </summary>
        [YamlMember(Alias = "shouldBeInOrder")]
        public bool ShouldBeInOrder { get; set; }

        /// <summary>
        /// Gets or sets whether it is allowed for the posted JSON to contain more fields than defined in the stub.
        /// </summary>
        [YamlMember(Alias = "cantContainExtraFields")]
        public bool CantContainExtraFields { get; set; }

        /// <summary>
        /// Gets or sets the JSON properties to check for.
        /// </summary>
        [YamlMember(Alias = "input")]
        public object Input { get; set; }
    }
}
