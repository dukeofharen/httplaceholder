using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model which contains the configuration needed to do a string or regex replace on the stub response.
/// </summary>
public class StubResponseReplaceDto : IMapFrom<StubResponseReplaceModel>, IMapTo<StubResponseReplaceModel>
{
    /// <summary>
    ///     The text to look for. Set either this or regex, not both.
    /// </summary>
    [YamlMember(Alias = "text")]
    public string Text { get; set; }

    /// <summary>
    ///     Whether to ignore the casing when looking for <see cref="Text" />.
    /// </summary>
    [YamlMember(Alias = "ignoreCase")]
    public bool? IgnoreCase { get; set; }

    /// <summary>
    ///     The regex expression to look for. Set either this or text, not both.
    /// </summary>
    [YamlMember(Alias = "regex")]
    public string Regex { get; set; }

    /// <summary>
    ///     The value the found text or regex matches should be replaced with.
    /// </summary>
    [YamlMember(Alias = "replaceWith")]
    public string ReplaceWith { get; set; }
}
