using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Export;

/// <summary>
///     Describes a class that is used to convert
/// </summary>
public interface IRequestToCurlCommandService
{
    /// <summary>
    ///     Converts a given request to a cURL command.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The cURL command.</returns>
    string Convert(RequestResultModel request);
}
