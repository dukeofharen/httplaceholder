using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A class for storing a stub with its metadata.
/// </summary>
public class FullStubDto : IMapFrom<FullStubModel>, IMapTo<FullStubModel>
{
    /// <summary>
    ///     Gets or sets the stub.
    /// </summary>
    [YamlMember(Alias = "stub")]
    public StubDto Stub { get; set; }

    /// <summary>
    ///     Gets or sets the metadata.
    /// </summary>
    [YamlMember(Alias = "metadata")]
    public StubMetadataDto Metadata { get; set; }
}
