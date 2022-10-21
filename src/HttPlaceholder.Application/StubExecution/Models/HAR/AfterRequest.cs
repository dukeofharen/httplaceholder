using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR AfterRequest.
/// </summary>
public class AfterRequest
{
    /// <summary>
    ///     Gets or sets the expires.
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
