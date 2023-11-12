namespace HttPlaceholder.Client.Dto.Enums;

/// <summary>
///     An enum to determine the export type of the request.
/// </summary>
public enum RequestExportType
{
    /// <summary>
    ///     Not set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    ///  cURL export.
    /// </summary>
    Curl = 1,

    /// <summary>
    ///     Hurl export.
    /// </summary>
    Hurl = 2,

    /// <summary>
    ///     HAR (HTTP Archive) export.
    /// </summary>
    Har = 3
}
