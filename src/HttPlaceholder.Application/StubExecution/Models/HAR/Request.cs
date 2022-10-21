using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Request.
/// </summary>
public class Request
{
    /// <summary>
    ///     Gets or sets method.
    /// </summary>
    [JsonProperty("method")]
    public string Method { get; set; }

    /// <summary>
    ///     Gets or sets url.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets http version.
    /// </summary>
    [JsonProperty("httpVersion")]
    public string HttpVersion { get; set; }

    /// <summary>
    ///     Gets or sets cookies.
    /// </summary>
    [JsonProperty("cookies")]
    public Cookie[] Cookies { get; set; }

    /// <summary>
    ///     Gets or sets headers.
    /// </summary>
    [JsonProperty("headers")]
    public Header[] Headers { get; set; }

    /// <summary>
    ///     Gets or sets query string.
    /// </summary>
    [JsonProperty("queryString")]
    public Query[] QueryString { get; set; }

    /// <summary>
    ///     Gets or sets post data.
    /// </summary>
    [JsonProperty("postData")]
    public PostData PostData { get; set; }

    /// <summary>
    ///     Gets or sets headers size.
    /// </summary>
    [JsonProperty("headersSize")]
    public int? HeadersSize { get; set; }

    /// <summary>
    ///     Gets or sets body size.
    /// </summary>
    [JsonProperty("bodySize")]
    public int BodySize { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
