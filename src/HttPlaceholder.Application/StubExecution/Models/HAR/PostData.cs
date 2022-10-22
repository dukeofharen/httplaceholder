using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR PostData.
/// </summary>
public class PostData
{
    /// <summary>
    ///     Gets or sets mime type.
    /// </summary>
    [JsonProperty("mimeType")]
    public string MimeType { get; set; }

    /// <summary>
    ///     Gets or sets text.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    ///     Gets or sets params.
    /// </summary>
    [JsonProperty("params")]
    public PostDataItem[] Params { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}

/// <summary>
///     The HAR PostDataItem.
/// </summary>
public class PostDataItem
{
    /// <summary>
    ///     Gets or sets name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets value.
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }

    /// <summary>
    ///     Gets or sets filename.
    /// </summary>
    [JsonProperty("fileName")]
    public string FileName { get; set; }

    /// <summary>
    ///     Gets or sets content type.
    /// </summary>
    [JsonProperty("contentType")]
    public string ContentType { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
