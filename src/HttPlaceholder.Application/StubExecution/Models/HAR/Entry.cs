using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
/// The HAR Entry.
/// </summary>
public class Entry
{
    /// <summary>
    /// Gets or sets pageref.
    /// </summary>
    [JsonProperty("pageref")]
    public string PageRef { get; set; }

    /// <summary>
    /// Gets or sets started date time.
    /// </summary>
    [JsonProperty("startedDateTime")]
    public DateTime StartedDateTime { get; set; }

    /// <summary>
    /// Gets or sets time.
    /// </summary>
    [JsonProperty("time")]
    public decimal Time { get; set; }

    /// <summary>
    /// Gets or sets request.
    /// </summary>
    [JsonProperty("request")]
    public Request Request { get; set; }

    /// <summary>
    /// Gets or sets response.
    /// </summary>
    [JsonProperty("response")]
    public Response Response { get; set; }

    /// <summary>
    /// Gets or sets cache.
    /// </summary>
    [JsonProperty("cache")]
    public Cache Cache { get; set; }

    /// <summary>
    /// Gets or sets timings.
    /// </summary>
    [JsonProperty("timings")]
    public Timings Timings { get; set; }

    /// <summary>
    /// Gets or sets server ip address.
    /// </summary>
    [JsonProperty("serverIPAddress")]
    public string ServerIpAddress { get; set; }

    /// <summary>
    /// Gets or sets connection.
    /// </summary>
    [JsonProperty("connection")]
    public string Connection { get; set; }

    /// <summary>
    /// Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
