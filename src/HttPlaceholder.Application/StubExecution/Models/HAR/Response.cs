﻿using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Response.
/// </summary>
public class Response
{
    /// <summary>
    ///     Gets or sets status.
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    ///     Gets or sets status text.
    /// </summary>
    [JsonProperty("statusText")]
    public string StatusText { get; set; }

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
    ///     Gets or sets content.
    /// </summary>
    [JsonProperty("content")]
    public Content Content { get; set; }

    /// <summary>
    ///     Gets or sets redirect url.
    /// </summary>
    [JsonProperty("redirectURL")]
    public string RedirectUrl { get; set; }

    /// <summary>
    ///     Gets or sets headers size.
    /// </summary>
    [JsonProperty("headersSize")]
    public int? HeadersSize { get; set; }

    /// <summary>
    ///     Gets or sets property.
    /// </summary>
    [JsonProperty("bodySize")]
    public int BodySize { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
