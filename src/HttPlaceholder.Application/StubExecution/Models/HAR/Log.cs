using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
/// The HAR Log.
/// </summary>
public class Log
{
    /// <summary>
    /// Gets or sets version.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets creator.
    /// </summary>
    [JsonProperty("creator")]
    public Creator Creator { get; set; }

    /// <summary>
    /// Gets or sets browser.
    /// </summary>
    [JsonProperty("browser")]
    public Browser Browser { get; set; }

    /// <summary>
    /// Gets or sets pages.
    /// </summary>
    [JsonProperty("pages")]
    public Page[] Pages { get; set; }

    /// <summary>
    /// Gets or sets entries.
    /// </summary>
    [JsonProperty("entries")]
    public Entry[] Entries { get; set; }

    /// <summary>
    /// Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
