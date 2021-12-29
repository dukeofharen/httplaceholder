using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Content
{
    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("compression")]
    public int Compression { get; set; }

    [JsonProperty("mimeType")]
    public string MimeType { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("encoding")]
    public string Encoding { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
