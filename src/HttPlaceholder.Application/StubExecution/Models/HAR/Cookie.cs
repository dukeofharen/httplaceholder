using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Cookie
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("domain")]
    public string Domain { get; set; }

    [JsonProperty("expires")]
    public DateTime? Expires { get; set; }

    [JsonProperty("httpOnly")]
    public bool HttpOnly { get; set; }

    [JsonProperty("secure")]
    public bool Secure { get; set; }
}
