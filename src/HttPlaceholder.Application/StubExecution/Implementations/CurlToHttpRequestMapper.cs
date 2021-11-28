using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc/>
    internal class CurlToHttpRequestMapper : ICurlToHttpRequestMapper
    {
        private readonly ILogger<CurlToHttpRequestMapper> _logger;

        public CurlToHttpRequestMapper(ILogger<CurlToHttpRequestMapper> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<HttpRequestModel> MapCurlCommandsToHttpRequest(string commands)
        {
            var result = new List<HttpRequestModel>();
            HttpRequestModel request = null;
            var parts = commands.Split(' ');
            if (parts.Length == 0)
            {
                _logger.LogDebug("cURL ommand string is empty, so not extracting request.");
                return result;
            }

            if (!IsCurl(parts[0]))
            {
                _logger.LogDebug("Command is not a cURL command, so not extracting request.");
                return result;
            }

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (IsCurl(part))
                {
                    request = new HttpRequestModel();
                    result.Add(request);
                    continue;
                }

                if (request == null)
                {
                    continue;
                }

                if (part == "-X")
                {
                    request.Method = parts[i + 1];
                    continue;
                }

                if (part == "-H")
                {
                    var header = ParseHeader(i, parts);
                    request.Headers.Add(header.key, header.value);
                    i = header.newNeedle;
                    continue;
                }

                // TODO check if "-X"
                // TODO check if "-H"
                // TODO check if "--data-raw"
                // TODO regex to check if part is URL
            }

            return result;
        }

        private static bool IsCurl(string part) => string.Equals(part, "curl", StringComparison.OrdinalIgnoreCase);

        private static (string key, string value, int newNeedle) ParseHeader(int needle, string[] parts)
        {
            var headerBuilder = new StringBuilder();
            var boundaryCharacter = parts[needle + 1][0];
            var escapedBoundaryCharacter = $@"\{boundaryCharacter}";
            var counter = needle + 1;
            var key = string.Empty;
            while (true)
            {
                var part = parts[counter];
                if (part[0] == boundaryCharacter && !part.StartsWith(escapedBoundaryCharacter))
                {
                    // The first part in a header string is the key.
                    key = new string(part.ToCharArray().Skip(1).ToArray()).TrimEnd(':');
                }
                else if (part[part.Length - 1] == boundaryCharacter && !part.EndsWith(escapedBoundaryCharacter))
                {
                    // The last part is found. Append it and break.
                    headerBuilder.Append(new string(part.ToCharArray().Take(part.Length - 1).ToArray()));
                    break;
                }
                else
                {
                    // Some part within.
                    headerBuilder.Append(part).Append(" ");
                }

                counter++;
            }

            headerBuilder.Replace(escapedBoundaryCharacter, boundaryCharacter.ToString());
            return (key, headerBuilder.ToString(), counter);
        }
    }
}
