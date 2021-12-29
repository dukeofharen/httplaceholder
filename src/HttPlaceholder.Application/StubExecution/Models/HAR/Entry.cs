using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Entry
{
    [JsonProperty("pageref")]
    public string PageRef { get; set; }

    [JsonProperty("startedDateTime")]
    public DateTime StartedDateTime { get; set; }

    [JsonProperty("time")]
    public decimal Time { get; set; }

    [JsonProperty("request")]
    public Request Request { get; set; }

    [JsonProperty("response")]
    public Response Response { get; set; }

    [JsonProperty("cache")]
    public Cache Cache { get; set; }

    [JsonProperty("timings")]
    public Timings Timings { get; set; }

    [JsonProperty("serverIPAddress")]
    public string ServerIpAddress { get; set; }

    [JsonProperty("connection")]
    public string Connection { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
