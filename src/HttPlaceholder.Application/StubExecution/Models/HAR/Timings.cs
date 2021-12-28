using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Timings
{
    [JsonProperty("dns")]
    public decimal Dns { get; set; }

    [JsonProperty("connect")]
    public decimal Connect { get; set; }

    [JsonProperty("blocked")]
    public decimal Blocked { get; set; }

    [JsonProperty("send")]
    public decimal Send { get; set; }

    [JsonProperty("wait")]
    public decimal Wait { get; set; }

    [JsonProperty("receive")]
    public decimal Receive { get; set; }

    [JsonProperty("ssl")]
    public decimal Ssl { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
