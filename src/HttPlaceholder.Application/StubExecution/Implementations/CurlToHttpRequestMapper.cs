using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc/>
    internal class CurlToHttpRequestMapper : ICurlToHttpRequestMapper
    {
        /// <inheritdoc/>
        public IEnumerable<StubModel> MapCurlCommandsToHttpRequest(string commands) =>
            throw new System.NotImplementedException();
    }
}
