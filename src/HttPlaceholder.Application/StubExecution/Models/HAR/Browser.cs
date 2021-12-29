using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Browser
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
