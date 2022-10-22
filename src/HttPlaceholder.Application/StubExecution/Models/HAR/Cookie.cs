using System;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

/// <summary>
///     The HAR Cookie.
/// </summary>
public class Cookie
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
    ///     Gets or sets path.
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    ///     Gets or sets domain.
    /// </summary>
    [JsonProperty("domain")]
    public string Domain { get; set; }

    /// <summary>
    ///     Gets or sets expires.
    /// </summary>
    [JsonProperty("expires")]
    public DateTime? Expires { get; set; }

    /// <summary>
    ///     Gets or sets http only.
    /// </summary>
    [JsonProperty("httpOnly")]
    public bool HttpOnly { get; set; }

    /// <summary>
    ///     Gets or sets secure.
    /// </summary>
    [JsonProperty("secure")]
    public bool Secure { get; set; }
}
