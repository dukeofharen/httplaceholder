namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
///     A model for storing metadata of a stub.
/// </summary>
public class StubMetadataDto
{
    /// <summary>
    ///     Gets or sets a value indicating whether [read only].
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    ///     Gets or sets the filename the stub is in
    /// </summary>
    public string Filename { get; set; }
}
