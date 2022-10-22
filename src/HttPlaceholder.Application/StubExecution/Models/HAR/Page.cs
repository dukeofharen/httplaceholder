using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Page.
/// </summary>
public class Page
{
    /// <summary>
    ///     Gets or sets started date time.
    /// </summary>
    [JsonProperty("startedDateTime")]
    public DateTime StartedDateTime { get; set; }

    /// <summary>
    ///     Gets or sets id.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets title.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets page timings.
    /// </summary>
    [JsonProperty("pageTimings")]
    public PageTimings PageTimings { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
