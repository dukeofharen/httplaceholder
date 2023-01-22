using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A class for storing a stripped down version of a stub with metadata.
/// </summary>
public class FullStubOverviewDto : IMapFrom<FullStubOverviewModel>
{
    /// <summary>
    ///     Gets or sets the stub.
    /// </summary>
    public StubOverviewDto Stub { get; set; }

    /// <summary>
    ///     Gets or sets the metadata.
    /// </summary>
    public StubMetadataDto Metadata { get; set; }
}
