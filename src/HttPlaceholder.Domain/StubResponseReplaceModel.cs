﻿using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model which contains the configuration needed to do a string or regex replace on the stub response.
/// </summary>
public class StubResponseReplaceModel
{
    /// <summary>
    ///     The text to look for. Set either this or regex, not both.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string Text { get; set; }

    /// <summary>
    ///     Whether to ignore the casing when looking for <see cref="Text" />.
    /// </summary>
    public bool? IgnoreCase { get; set; }

    /// <summary>
    ///     The regex expression to look for. Set either this or text, not both.
    /// </summary>
    [YamlMember(Alias = "regex")]
    public string Regex { get; set; }

    /// <summary>
    ///     The JSONPath expression that should be looked for in the JSON.
    /// </summary>
    public string JsonPath { get; set; }

    /// <summary>
    ///     The value the found text or regex matches should be replaced with.
    /// </summary>
    [YamlMember(Alias = "replaceWith")]
    public string ReplaceWith { get; set; }
}
