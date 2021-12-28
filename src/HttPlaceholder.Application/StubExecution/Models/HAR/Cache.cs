using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Cache
{
    [JsonProperty("beforeRequest")]
    public BeforeRequest BeforeRequest { get; set; }

    [JsonProperty("afterRequest")]
    public AfterRequest AfterRequest { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
