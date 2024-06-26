using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Dto.v1.Metadata;

/// <summary>
///     A model that is used for displaying what types of variable handlers are available.
/// </summary>
public class VariableHandlerDto : IMapFrom<VariableHandlerModel>
{
    /// <summary>
    ///     Gets or sets the name of the variable handler.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the full name of the variable handler.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    ///     Gets or sets a short instruction on how to use the variable handler.
    /// </summary>
    public string Example { get; set; }

    /// <summary>
    ///     Gets or sets one or more examples.
    /// </summary>
    public string[] Examples { get; set; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    public string Description { get; set; }
}
