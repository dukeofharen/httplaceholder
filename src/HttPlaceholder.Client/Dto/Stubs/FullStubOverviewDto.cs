namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
/// A class for storing a stripped down version of a stub with metadata.
/// </summary>
public class FullStubOverviewDto
{
    /// <summary>
    /// Gets or sets the stub.
    /// </summary>
    public StubOverviewDto Stub { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public StubMetadataDto Metadata { get; set; }
}