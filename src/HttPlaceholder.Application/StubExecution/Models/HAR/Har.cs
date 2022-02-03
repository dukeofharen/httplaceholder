using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
/// The HAR.
/// </summary>
public class Har
{
    /// <summary>
    /// Gets or sets log.
    /// </summary>
    [JsonProperty("log")]
    public Log Log { get; set; }
}
