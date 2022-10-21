namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
///     A class for storing a stub with its metadata.
/// </summary>
public class FullStubDto
{
    /// <summary>
    ///     Gets or sets the stub.
    /// </summary>
    public StubDto Stub { get; set; }

    /// <summary>
    ///     Gets or sets the metadata.
    /// </summary>
    public StubMetadataDto Metadata { get; set; }
}
