using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc/>
    internal class CurlToHttpRequestMapper : ICurlToHttpRequestMapper
    {
        private static readonly Regex _urlRegex =
            new(
                @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)",
                RegexOptions.Compiled);

        private readonly ILogger<CurlToHttpRequestMapper> _logger;

        public CurlToHttpRequestMapper(ILogger<CurlToHttpRequestMapper> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<HttpRequestModel> MapCurlCommandsToHttpRequest(string commands)
        {
            void SetMethod(HttpRequestModel httpRequestModel)
            {
                // If no specific HTTP method has been provided to cURL and the request has no body,
                // the method will be GET, otherwise it will be POST.
                httpRequestModel.Method = httpRequestModel.Body == null ? "GET" : "POST";
            }

            var result = new List<HttpRequestModel>();
            HttpRequestModel request = null;
            var parts = commands.Trim().Split(' ');
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
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                if (IsCurl(part))
                {
                    if (request != null)
                    {
                        SetMethod(request);
                    }

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

                if (string.IsNullOrWhiteSpace(request.Body) && part == "--data-raw")
                {
                    var body = ParseBody(i, parts);
                    request.Body = body.body;
                    i = body.newNeedle;
                    continue;
                }

                // TODO add logging
                // TODO check "--compressed" (sets Accept-Encoding header to "deflate, gzip, br" apparently)
                if (string.IsNullOrWhiteSpace(request.Url) && _urlRegex.IsMatch(part))
                {
                    var matches = _urlRegex.Matches(part).Cast<Match>().ToArray();
                    if (matches.Length == 1)
                    {
                        request.Url = matches[0].Value;
                    }

                    continue;
                }
            }

            foreach (var item in result.Where(r => r.Method == null))
            {
                SetMethod(item);
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

        private static (string body, int newNeedle) ParseBody(int needle, string[] parts)
        {
            var boundaryCharacter = parts[needle + 1][0];
            var escapedBoundaryCharacter = $@"\{boundaryCharacter}";

            bool PartStartsWithBoundaryChar(string part) =>
                part[0] == boundaryCharacter && !part.StartsWith(escapedBoundaryCharacter);

            bool PartEndsWithBoundaryChar(string part) => part[part.Length - 1] == boundaryCharacter &&
                                                          !part.EndsWith(escapedBoundaryCharacter);

            var bodyBuilder = new StringBuilder(); // Hah nice
            var counter = needle + 1;
            while (true)
            {
                if (counter >= parts.Length)
                {
                    break;
                }

                var part = parts[counter];
                if (PartStartsWithBoundaryChar(part) && PartEndsWithBoundaryChar(part))
                {
                    // This body consists of one part. Just strip the boundary characters and we're good to go.
                    bodyBuilder.Append(new string(part.ToCharArray().Skip(1).Take(part.Length - 2).ToArray()));
                    break;
                }

                if (PartStartsWithBoundaryChar(part))
                {
                    // The first part is found. Remove the boundary character and continue.
                    bodyBuilder.Append(new string(part.ToCharArray().Skip(1).ToArray()));
                }
                else if (PartEndsWithBoundaryChar(part))
                {
                    // The last part is found. Append it and break.
                    bodyBuilder.Append(new string(part.ToCharArray().Take(part.Length - 1).ToArray()));
                    break;
                }
                else
                {
                    // Some part within.
                    bodyBuilder.Append(part).Append(" ");
                }

                counter++;
            }

            bodyBuilder.Replace(escapedBoundaryCharacter, boundaryCharacter.ToString());
            return (bodyBuilder.ToString(), counter);
        }
    }
}
