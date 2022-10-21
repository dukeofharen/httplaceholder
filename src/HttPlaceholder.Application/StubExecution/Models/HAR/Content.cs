using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Content.
/// </summary>
public class Content
{
    /// <summary>
    ///     Gets or sets size.
    /// </summary>
    [JsonProperty("size")]
    public int Size { get; set; }

    /// <summary>
    ///     Gets or sets compression.
    /// </summary>
    [JsonProperty("compression")]
    public int Compression { get; set; }

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
    ///     Gets or sets encoding.
    /// </summary>
    [JsonProperty("encoding")]
    public string Encoding { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
