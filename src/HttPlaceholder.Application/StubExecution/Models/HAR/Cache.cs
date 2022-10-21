using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Cache.
/// </summary>
public class Cache
{
    /// <summary>
    ///     Gets or sets before request.
    /// </summary>
    [JsonProperty("beforeRequest")]
    public BeforeRequest BeforeRequest { get; set; }

    /// <summary>
    ///     Gets or sets after request.
    /// </summary>
    [JsonProperty("afterRequest")]
    public AfterRequest AfterRequest { get; set; }

    /// <summary>
    ///     Gets or sets comment.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}
