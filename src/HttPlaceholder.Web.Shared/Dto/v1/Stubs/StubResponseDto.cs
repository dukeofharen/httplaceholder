using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Web.Shared.Attributes;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing all possible response parameters for a stub.
/// </summary>
[CustomOpenApi]
public class StubResponseDto : IMapFrom<StubResponseModel>, IMapTo<StubResponseModel>
{
    /// <summary>
    ///     Gets or sets whether dynamic mode is on.
    /// </summary>
    [YamlMember(Alias = "enableDynamicMode")]
    public bool? EnableDynamicMode { get; set; }

    /// <summary>
    ///     Gets or sets the status code.
    /// </summary>
    [YamlMember(Alias = "statusCode")]
    public int? StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the response content type.
    /// </summary>
    [YamlMember(Alias = "contentType")]
    public string ContentType { get; set; }

    /// <summary>
    ///     Gets or sets the text.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string Text { get; set; }

    /// <summary>
    ///     Gets or sets the base64.
    /// </summary>
    [YamlMember(Alias = "base64")]
    public string Base64 { get; set; }

    /// <summary>
    ///     Gets or sets the file.
    /// </summary>
    [YamlMember(Alias = "file")]
    public string File { get; set; }

    /// <summary>
    ///     Gets or sets the text file.
    /// </summary>
    [YamlMember(Alias = "textFile")]
    public string TextFile { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    [YamlMember(Alias = "headers")]
    public IDictionary<string, string> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the duration of the extra.
    /// </summary>
    [YamlMember(Alias = "extraDuration")]
    [OneOf(Types = new[] {typeof(int), typeof(StubExtraDurationDto)})]
    public object ExtraDuration { get; set; }

    /// <summary>
    ///     Gets or sets the json.
    /// </summary>
    [YamlMember(Alias = "json")]
    public string Json { get; set; }

    /// <summary>
    ///     Gets or sets the XML.
    /// </summary>
    [YamlMember(Alias = "xml")]
    public string Xml { get; set; }

    /// <summary>
    ///     Gets or sets the HTML.
    /// </summary>
    [YamlMember(Alias = "html")]
    public string Html { get; set; }

    /// <summary>
    ///     Gets or sets the temporary redirect.
    /// </summary>
    [YamlMember(Alias = "temporaryRedirect")]
    public string TemporaryRedirect { get; set; }

    /// <summary>
    ///     Gets or sets the permanent redirect.
    /// </summary>
    [YamlMember(Alias = "permanentRedirect")]
    public string PermanentRedirect { get; set; }

    /// <summary>
    ///     Gets or sets moved permanently.
    /// </summary>
    [YamlMember(Alias = "movedPermanently")]
    public string MovedPermanently { get; set; }

    /// <summary>
    ///     Gets or sets the reverse proxy settings.
    /// </summary>
    [YamlMember(Alias = "reverseProxy")]
    public StubResponseReverseProxyDto ReverseProxy { get; set; }

    /// <summary>
    ///     Gets or sets the line endings type.
    /// </summary>
    [YamlMember(Alias = "lineEndings")]
    public LineEndingType? LineEndings { get; set; }

    /// <summary>
    ///     Gets or sets the stub image.
    /// </summary>
    [YamlMember(Alias = "image")]
    public StubResponseImageDto Image { get; set; }

    /// <summary>
    ///     Gets or sets the response scenario variables.
    /// </summary>
    [YamlMember(Alias = "scenario")]
    public StubResponseScenarioDto Scenario { get; set; }

    /// <summary>
    ///     Gets or sets whether the connection should be aborted.
    /// </summary>
    [YamlMember(Alias = "abortConnection")]
    public bool AbortConnection { get; set; }

    /// <summary>
    ///     Gets or sets a list of text or regexes with a corresponding value that should be used to do a replace in the stub
    ///     response.
    /// </summary>
    [YamlMember(Alias = "replace")]
    public IEnumerable<StubResponseReplaceDto> Replace { get; set; }
}
