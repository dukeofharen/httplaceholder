using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR BeforeRequest.
/// </summary>
public class BeforeRequest
{
    /// <summary>
    ///     Gets or sets expires.
    /// </summary>
    [JsonProperty("expires")]
    public DateTime Expires { get; set; }

    /// <summary>
    ///     Gets or sets last access.
    /// </summary>
    [JsonProperty("lastAccess")]
    public DateTime LastAccess { get; set; }

    /// <summary>
    ///     Gets or sets etag.
    /// </summary>
    [JsonProperty("eTag")]
    public string ETag { get; set; }

    /// <summary>
    ///     Gets or sets hit count.
    /// </summary>
    [JsonProperty("hitCount")]
    public int HitCount { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
