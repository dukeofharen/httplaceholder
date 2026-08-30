namespace HttPlaceholder.Domain;

/// <summary>
///     A class for storing a stripped down version of a stub with metadata.
/// </summary>
public class FullStubOverviewModel
{
    /// <summary>
    ///     Gets or sets the stub.
    /// </summary>
    public StubOverviewModel Stub { get; set; }

    /// <summary>
    ///     Gets or sets the metadata.
    /// </summary>
    public StubMetadataModel Metadata { get; set; }
}
