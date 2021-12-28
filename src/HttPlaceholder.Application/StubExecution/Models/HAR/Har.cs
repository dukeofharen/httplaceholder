using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Har
{
    [JsonProperty("log")]
    public Log Log { get; set; }
}
