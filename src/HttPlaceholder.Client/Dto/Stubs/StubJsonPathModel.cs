namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
///     A model for storing data for the JSONPath condition checker.
/// </summary>
public class StubJsonPathModel
{
    /// <summary>
    ///     Gets or sets the JSONPath query.
    /// </summary>
    public string Query { get; set; }

    /// <summary>
    ///     Gets or sets the expected value.
    /// </summary>
    public string ExpectedValue { get; set; }
}
