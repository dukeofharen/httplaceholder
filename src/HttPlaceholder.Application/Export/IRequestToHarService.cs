using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Export;

/// <summary>
///     Describes a class that is used to convert a request to an HTTP archive (HAR) file.
/// </summary>
public interface IRequestToHarService
{
    /// <summary>
    ///     Converts a request to an HTTP archive (HAR) file.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <returns>The HAR JSON.</returns>
    string Convert(RequestResultModel request, ResponseModel response);
}
