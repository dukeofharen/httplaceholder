using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class AfterRequest
{
    [JsonProperty("expires")]
    public DateTime Expires { get; set; }

    [JsonProperty("lastAccess")]
    public DateTime LastAccess { get; set; }

    [JsonProperty("eTag")]
    public string ETag { get; set; }

    [JsonProperty("hitCount")]
    public int HitCount { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
