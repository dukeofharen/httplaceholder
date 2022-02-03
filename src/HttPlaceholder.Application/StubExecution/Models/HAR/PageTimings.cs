using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
/// The HAR PageTimings.
/// </summary>
public class PageTimings
{
    /// <summary>
    /// Gets or sets on content load.
    /// </summary>
    [JsonProperty("onContentLoad")]
    public decimal OnContentLoad { get; set; }

    /// <summary>
    /// Gets or sets on load.
    /// </summary>
    [JsonProperty("onLoad")]
    public decimal OnLoad { get; set; }

    /// <summary>
    /// Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
