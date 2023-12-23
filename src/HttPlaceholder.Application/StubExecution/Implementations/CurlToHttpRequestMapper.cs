using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class CurlToHttpRequestMapper(ILogger<CurlToHttpRequestMapper> logger)
    : ICurlToHttpRequestMapper, ISingletonService
{
    private static readonly Regex _urlRegex =
        new(
            @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}(\.[a-zA-Z0-9()]{1,6})?\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)",
            RegexOptions.Compiled);

    /// <inheritdoc />
    public IEnumerable<HttpRequestModel> MapCurlCommandsToHttpRequest(string commands)
    {
        void SetMethod(HttpRequestModel httpRequestModel)
        {
            // If no specific HTTP method has been provided to cURL and the request has no body,
            // the method will be GET, otherwise it will be POST.
            httpRequestModel.Method = httpRequestModel.Body == null ? "GET" : "POST";
        }

        try
        {
            var result = new List<HttpRequestModel>();
            HttpRequestModel request = null;
            if (string.IsNullOrWhiteSpace(commands))
            {
                logger.LogDebug("cURL command string is empty, so not extracting request.");
                return result;
            }

            var parts = commands.Trim().Split(new[] {" ", "\r\n", "\n"}, StringSplitOptions.None);
            if (!IsCurl(parts[0]))
            {
                logger.LogDebug("Command is not a cURL command, so not extracting request.");
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
                    request.Method = ParseRequestMethod(parts, i);
                    continue;
                }

                if (part == "-H")
                {
                    var (key, value, newNeedle) = ParseHeader(i, parts);
                    request.Headers.Add(key, value);
                    i = newNeedle;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(request.Body) &&
                    part is "--data-raw" or "-F" or "--form" or "-d" or "--data")
                {
                    switch (part)
                    {
                        case "-F" or "--form":
                            request.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType,
                                MimeTypes.MultipartFormDataMime);
                            break;
                        case "-d" or "--data":
                            request.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType,
                                MimeTypes.UrlEncodedFormMime);
                            break;
                    }

                    var (body, newNeedle) = ParseBody(i, parts);
                    request.Body = body;
                    i = newNeedle;
                    continue;
                }

                if (part is "-u" or "--user")
                {
                    var auth = ParseBasicAuthFlag(parts, i);
                    if (!string.IsNullOrWhiteSpace(auth))
                    {
                        request.Headers.AddOrReplaceCaseInsensitive("Authorization", auth);
                    }
                }

                if (part == "--compressed")
                {
                    if (string.IsNullOrWhiteSpace(request.Headers.CaseInsensitiveSearch(HeaderKeys.AcceptEncoding)))
                    {
                        request.Headers.Add(HeaderKeys.AcceptEncoding, "deflate, gzip, br");
                    }

                    continue;
                }

                if (!string.IsNullOrWhiteSpace(request.Url) || !_urlRegex.IsMatch(part))
                {
                    continue;
                }

                var matches = _urlRegex.Matches(part).ToArray();
                if (matches.Length == 1)
                {
                    request.Url = matches[0].Value;
                }
            }

            foreach (var item in result.Where(r => r.Method == null))
            {
                SetMethod(item);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception occurred while trying to parse cURL commands.");
            throw new ValidationException(new[]
            {
                $"Exception occurred while trying to parse cURL commands: {ex.Message}"
            });
        }
    }

    private static string ParseRequestMethod(string[] parts, int needle)
    {
        var method = parts[needle + 1];
        var firstChar = method.ToCharArray()[0];
        if (!char.IsLetter(firstChar))
        {
            method = method.Trim(firstChar);
        }

        return method;
    }

    private static string ParseBasicAuthFlag(string[] parts, int needle)
    {
        var auth = parts[needle + 1];
        var firstChar = auth.ToCharArray()[0];
        if (!char.IsLetter(firstChar))
        {
            auth = auth.Trim(firstChar);
        }

        var authParts = auth.Split(':');
        return authParts.Length != 2
            ? string.Empty
            : $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authParts[0]}:{authParts[1]}"))}";
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
            counter++;
            if (string.IsNullOrEmpty(part))
            {
                continue;
            }

            if (part[0] == boundaryCharacter && !part.StartsWith(escapedBoundaryCharacter) && part.EndsWith(":"))
            {
                // The first part in a header string is the key.
                key = new string(part.ToCharArray().Skip(1).ToArray()).TrimEnd(':');
            }
            else if (part[^1] == boundaryCharacter && !part.EndsWith(escapedBoundaryCharacter))
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
        }

        headerBuilder.Replace(escapedBoundaryCharacter, boundaryCharacter.ToString());
        return (key, headerBuilder.ToString(), counter - 1);
    }

    private static (string body, int newNeedle) ParseBody(int needle, IEnumerable<string> parts)
    {
        var extractedParts = parts.Skip(needle + 1).ToArray();
        var boundaryCharacter = extractedParts[0][0];
        if (boundaryCharacter == '$')
        {
            // Apparently, in some cases, a "$" is prefixed before the actual body, so we need to strip it before determining the actual boundary character.
            extractedParts[0] = extractedParts[0].TrimStart('$');
            boundaryCharacter = extractedParts[0][0];
        }

        var escapedBoundaryCharacter = $@"\{boundaryCharacter}";

        bool PartStartsWithBoundaryChar(string part)
        {
            return !string.IsNullOrWhiteSpace(part) && part[0] == boundaryCharacter &&
                   !part.StartsWith(escapedBoundaryCharacter);
        }

        bool PartEndsWithBoundaryChar(string part)
        {
            return !string.IsNullOrWhiteSpace(part) &&
                   part[^1] == boundaryCharacter &&
                   !part.EndsWith(escapedBoundaryCharacter);
        }

        var bodyBuilder = new StringBuilder(); // Hah nice
        var counter = 0;
        while (true)
        {
            if (counter >= extractedParts.Length)
            {
                break;
            }

            var part = extractedParts[counter];
            if (PartStartsWithBoundaryChar(part) && PartEndsWithBoundaryChar(part))
            {
                // This body consists of one part. Just strip the boundary characters and we're good to go.
                bodyBuilder.Append(new string(part.ToCharArray().Skip(1).Take(part.Length - 2).ToArray()));
                break;
            }

            if (PartStartsWithBoundaryChar(part))
            {
                // The first part is found. Remove the boundary character and continue.
                bodyBuilder.Append(new string(part.ToCharArray().Skip(1).ToArray())).Append(" ");
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
        return (bodyBuilder.ToString(), needle + counter);
    }
}
