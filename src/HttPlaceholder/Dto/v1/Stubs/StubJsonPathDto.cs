using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.v1.Stubs;

/// <summary>
///     A model for storing data for the JSONPath condition checker.
/// </summary>
public class StubJsonPathDto
{
    /// <summary>
    ///     Gets or sets the JSONPath query.
    /// </summary>
    [YamlMember(Alias = "query")]
    [JsonProperty("query")]
    public string Query { get; set; }

    /// <summary>
    ///     Gets or sets the expected value.
    /// </summary>
    [YamlMember(Alias = "expectedValue")]
    [JsonProperty("expectedValue")]
    public string ExpectedValue { get; set; }
}
