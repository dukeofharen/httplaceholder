using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Log
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("creator")]
    public Creator Creator { get; set; }

    [JsonProperty("browser")]
    public Browser Browser { get; set; }

    [JsonProperty("pages")]
    public Page[] Pages { get; set; }

    [JsonProperty("entries")]
    public Entry[] Entries { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
