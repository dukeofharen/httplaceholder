using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
/// The HAR Timings.
/// </summary>
public class Timings
{
    /// <summary>
    /// Gets or sets dns.
    /// </summary>
    [JsonProperty("dns")]
    public decimal Dns { get; set; }

    /// <summary>
    /// Gets or sets connect.
    /// </summary>
    [JsonProperty("connect")]
    public decimal Connect { get; set; }

    /// <summary>
    /// Gets or sets blocked.
    /// </summary>
    [JsonProperty("blocked")]
    public decimal Blocked { get; set; }

    /// <summary>
    /// Gets or sets send.
    /// </summary>
    [JsonProperty("send")]
    public decimal Send { get; set; }

    /// <summary>
    /// Gets or sets wait.
    /// </summary>
    [JsonProperty("wait")]
    public decimal Wait { get; set; }

    /// <summary>
    /// Gets or sets receive.
    /// </summary>
    [JsonProperty("receive")]
    public decimal Receive { get; set; }

    /// <summary>
    /// Gets or sets ssl.
    /// </summary>
    [JsonProperty("ssl")]
    public decimal Ssl { get; set; }

    /// <summary>
    /// Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
