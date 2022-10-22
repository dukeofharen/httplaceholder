namespace HttPlaceholder.Dto.v1.Requests;

/// <summary>
///     A model which contains metadata which is used when creating a stub from a request.
/// </summary>
public class CreateStubForRequestInputDto
{
    /// <summary>
    ///     Gets or sets whether to add the stub to the data source. If set to false, the stub is only returned but not added.
    /// </summary>
    public bool DoNotCreateStub { get; set; }
}
