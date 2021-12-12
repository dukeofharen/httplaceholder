using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.v1.Stubs;

/// <summary>
/// A model for storing data for the form condition checker.
/// </summary>
public class StubFormDto : IMapFrom<StubFormModel>, IMapTo<StubFormModel>
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    [YamlMember(Alias = "key")]
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [YamlMember(Alias = "value")]
    public string Value { get; set; }
}