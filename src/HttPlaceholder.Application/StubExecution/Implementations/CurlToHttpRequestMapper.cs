using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc/>
    internal class CurlToHttpRequestMapper : ICurlToHttpRequestMapper
    {
        /// <inheritdoc/>
        public IEnumerable<HttpRequestModel> MapCurlCommandsToHttpRequest(string commands)
        {
            return null;
        }
    }
}
