using System.Collections.Generic;
using HttPlaceholder.Client.Dto.Enums;

namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
///     A model for storing all possible response parameters for a stub.
/// </summary>
public class StubResponseDto
{
    /// <summary>
    ///     Gets or sets whether dynamic mode is on.
    /// </summary>
    public bool? EnableDynamicMode { get; set; }

    /// <summary>
    ///     Gets or sets the status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the response content type.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    ///     Gets or sets the text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Gets or sets the base64.
    /// </summary>
    public string Base64 { get; set; }

    /// <summary>
    ///     Gets or sets the file.
    /// </summary>
    public string File { get; set; }

    /// <summary>
    ///     Gets or sets the text file.
    /// </summary>
    public string TextFile { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the duration of the extra.
    /// </summary>
    public object ExtraDuration { get; set; }

    /// <summary>
    ///     Gets or sets the json.
    /// </summary>
    public string Json { get; set; }

    /// <summary>
    ///     Gets or sets the XML.
    /// </summary>
    public string Xml { get; set; }

    /// <summary>
    ///     Gets or sets the HTML.
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    ///     Gets or sets the temporary redirect.
    /// </summary>
    public string TemporaryRedirect { get; set; }

    /// <summary>
    ///     Gets or sets the permanent redirect.
    /// </summary>
    public string PermanentRedirect { get; set; }

    /// <summary>
    ///     Gets or sets the reverse proxy settings.
    /// </summary>
    public StubResponseReverseProxyDto ReverseProxy { get; set; }

    /// <summary>
    ///     Gets or sets the line endings type.
    /// </summary>
    public LineEndingType? LineEndings { get; set; }

    /// <summary>
    ///     Gets or sets the stub image.
    /// </summary>
    public StubResponseImageDto Image { get; set; }

    /// <summary>
    ///     Gets or sets the response scenario variables.
    /// </summary>
    public StubResponseScenarioDto Scenario { get; set; }

    /// <summary>
    ///     Gets or sets whether the connection should be aborted.
    /// </summary>
    public bool AbortConnection { get; set; }
}
