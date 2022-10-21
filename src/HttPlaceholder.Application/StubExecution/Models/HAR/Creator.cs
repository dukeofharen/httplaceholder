using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Creator.
/// </summary>
public class Creator
{
    /// <summary>
    ///     Gets or sets name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets version.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
