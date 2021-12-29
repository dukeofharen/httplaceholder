using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class Response
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("statusText")]
    public string StatusText { get; set; }

    [JsonProperty("httpVersion")]
    public string HttpVersion { get; set; }

    [JsonProperty("cookies")]
    public Cookie[] Cookies { get; set; }

    [JsonProperty("headers")]
    public Header[] Headers { get; set; }

    [JsonProperty("content")]
    public Content Content { get; set; }

    [JsonProperty("redirectURL")]
    public string RedirectURL { get; set; }

    [JsonProperty("headersSize")]
    public int HeadersSize { get; set; }

    [JsonProperty("bodySize")]
    public int BodySize { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
