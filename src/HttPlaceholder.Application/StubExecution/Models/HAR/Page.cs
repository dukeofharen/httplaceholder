using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Page
{
    [JsonProperty("startedDateTime")]
    public DateTime StartedDateTime { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("pageTimings")]
    public PageTimings PageTimings { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
