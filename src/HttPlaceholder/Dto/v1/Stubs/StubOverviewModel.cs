using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Dto.v1.Stubs
{
    /// <summary>
    /// A class for storing a stripped down version of a stub.
    /// </summary>
    public class StubOverviewDto : IMapFrom<StubOverviewModel>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string Tenant { get; set; }
    }
}
