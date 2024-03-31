using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Attributes;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing data for the form condition checker.
/// </summary>
[CustomOpenApi]
public class StubFormDto : IMapFrom<StubFormModel>, IMapTo<StubFormModel>
{
    /// <summary>
    ///     Gets or sets the key.
    /// </summary>
    [YamlMember(Alias = "key")]
    public string Key { get; set; }

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    [YamlMember(Alias = "value")]
    [OneOf(Types = [typeof(string), typeof(StubConditionStringCheckingDto)])]
    public object Value { get; set; }
}
