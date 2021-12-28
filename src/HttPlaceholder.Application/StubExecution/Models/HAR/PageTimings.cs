using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class PageTimings
{
    [JsonProperty("onContentLoad")]
    public decimal OnContentLoad { get; set; }

    [JsonProperty("onLoad")]
    public decimal OnLoad { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
