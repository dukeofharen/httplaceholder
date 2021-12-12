using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
/// A model for storing data for the JSONPath condition checker.
/// </summary>
public class StubJsonPathModel
{
    /// <summary>
    /// Gets or sets the JSONPath query.
    /// </summary>
    [YamlMember(Alias = "query")]
    public string Query { get; set; }

    /// <summary>
    /// Gets or sets the expected value.
    /// </summary>
    [YamlMember(Alias = "expectedValue")]
    public string ExpectedValue { get; set; }
}