using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    /// <summary>
    /// Describes a class that is being used to convert one or multiple cURL commands to <see cref="HttpRequestModel"/>.
    /// </summary>
    public interface ICurlToHttpRequestMapper
    {
        /// <summary>
        /// Converts one or multiple cURL commands to <see cref="HttpRequestModel"/>.
        /// </summary>
        /// <param name="commands">The cURL command(s).</param>
        /// <returns>A list of <see cref="StubModel"/>.</returns>
        IEnumerable<StubModel> MapCurlCommandsToHttpRequest(string commands);
    }
}
