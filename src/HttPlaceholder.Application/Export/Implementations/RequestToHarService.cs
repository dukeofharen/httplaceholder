using System;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Cookie = HttPlaceholder.Application.StubExecution.Models.HAR.Cookie;
using ConvertUtil = System.Convert;

namespace HttPlaceholder.Application.Export.Implementations;

internal class RequestToHarService(IAssemblyService assemblyService) : IRequestToHarService, ISingletonService
{
    public string Convert(RequestResultModel request, ResponseModel response)
    {
        var requestParams = request.RequestParameters;
        var requestContentType = requestParams.Headers.CaseInsensitiveSearch(HeaderKeys.ContentType);
        var responseContentType = response.Headers.CaseInsensitiveSearch(HeaderKeys.ContentType);
        var responseBody = response.Body ?? [];
        var uri = new Uri(requestParams.Url);
        var query = QueryHelpers.ParseQuery(uri.Query);
        var pageId = $"page_{request.CorrelationId}";
        var version = assemblyService.GetAssemblyVersion();
        const string httpVersion = "HTTP/1.1";
        var har = new Har
        {
            Log = new Log
            {
                Version = "1.2",
                Creator = new Creator { Name = "HttPlaceholder", Version = version },
                Pages =
                [
                    new Page
                    {
                        Id = pageId,
                        StartedDateTime = request.RequestBeginTime,
                        Title = string.Empty,
                        PageTimings = new PageTimings()
                    }
                ],
                Entries =
                [
                    new Entry
                    {
                        PageRef = pageId,
                        StartedDateTime = request.RequestBeginTime,
                        Request = new Request
                        {
                            Method = requestParams.Method,
                            Url = requestParams.Url,
                            HttpVersion = httpVersion,
                            Headers =
                                requestParams.Headers
                                    .Select(h => new Header { Name = h.Key, Value = h.Value }).ToArray(),
                            Cookies = Array.Empty<Cookie>(),
                            PostData = new PostData
                            {
                                MimeType = requestContentType,
                                Text =
                                    Encoding.UTF8.GetString(requestParams.BinaryBody ??
                                                            Array.Empty<byte>())
                            },
                            QueryString =
                                query.Select(q => new Query { Name = q.Key, Value = q.Value.FirstOrDefault() })
                                    .ToArray()
                        },
                        Response = new Response
                        {
                            Status = response.StatusCode,
                            StatusText = ReasonPhrases.GetReasonPhrase(response.StatusCode),
                            HttpVersion = httpVersion,
                            Headers =
                                response.Headers.Select(h => new Header { Name = h.Key, Value = h.Value })
                                    .ToArray(),
                            Cookies = Array.Empty<Cookie>(),
                            Content = new Content
                            {
                                Encoding = response.BodyIsBinary ? "base64" : null,
                                MimeType = responseContentType,
                                Size = responseBody.Length,
                                Text = response.BodyIsBinary
                                    ? ConvertUtil.ToBase64String(responseBody)
                                    : Encoding.UTF8.GetString(responseBody)
                            },
                            RedirectUrl = string.Empty
                        },
                        Cache = new Cache(),
                        Timings = new Timings(),
                        Time = 0,
                        ServerIpAddress = string.Empty,
                        Connection = string.Empty
                    }
                ]
            }
        };
        return JsonConvert.SerializeObject(har,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }
}
