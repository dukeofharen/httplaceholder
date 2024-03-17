using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Attributes;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing information about the URL condition checkers.
/// </summary>
[CustomOpenApi]
public class StubUrlConditionDto : IMapFrom<StubUrlConditionModel>, IMapTo<StubUrlConditionModel>
{
    /// <summary>
    ///     Gets or sets the path.
    /// </summary>
    [YamlMember(Alias = "path")]
    [OneOf(Types = new[] { typeof(string), typeof(StubConditionStringCheckingDto) })]
    public object Path { get; set; }

    /// <summary>
    ///     Gets or sets the query.
    /// </summary>
    [YamlMember(Alias = "query")]
    [OneOf(AdditionalPropertiesTypes = new[] { typeof(string), typeof(StubConditionStringCheckingDto) })]
    public IDictionary<string, object> Query { get; set; }

    /// <summary>
    ///     Gets or sets the full path.
    /// </summary>
    [YamlMember(Alias = "fullPath")]
    [OneOf(Types = new[] { typeof(string), typeof(StubConditionStringCheckingDto) })]
    public object FullPath { get; set; }

    /// <summary>
    ///     Gets or sets the is HTTPS.
    /// </summary>
    [YamlMember(Alias = "isHttps")]
    public bool? IsHttps { get; set; }
}
