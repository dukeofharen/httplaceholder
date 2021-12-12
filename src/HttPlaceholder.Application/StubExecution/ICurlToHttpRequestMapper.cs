using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is being used to convert one or multiple cURL commands to <see cref="HttpRequestModel"/>.
/// </summary>
public interface ICurlToHttpRequestMapper
{
    /// <summary>
    /// Converts one or multiple cURL commands to <see cref="HttpRequestModel"/>.
    /// </summary>
    /// <param name="commands">The cURL command(s).</param>
    /// <returns>A list of <see cref="HttpRequestModel"/>.</returns>
    IEnumerable<HttpRequestModel> MapCurlCommandsToHttpRequest(string commands);
}