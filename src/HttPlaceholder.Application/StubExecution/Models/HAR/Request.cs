using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Request
{
    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("httpVersion")]
    public string HttpVersion { get; set; }

    [JsonProperty("cookies")]
    public Cookie[] Cookies { get; set; }

    [JsonProperty("headers")]
    public Header[] Headers { get; set; }

    [JsonProperty("queryString")]
    public Query[] QueryString { get; set; }

    [JsonProperty("postData")]
    public PostData PostData { get; set; }

    [JsonProperty("headersSize")]
    public int HeadersSize { get; set; }

    [JsonProperty("bodySize")]
    public int BodySize { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
