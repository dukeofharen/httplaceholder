using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Web.Shared.Dto.v1.Export;

/// <summary>
///     A class which
/// </summary>
public class RequestExportResultDto
{
    /// <summary>
    ///     Gets or sets the request export type.
    /// </summary>
    public RequestExportType RequestExportType { get; set; }

    /// <summary>
    ///     Gets or sets the result.
    /// </summary>
    public string Result { get; set; }
}
