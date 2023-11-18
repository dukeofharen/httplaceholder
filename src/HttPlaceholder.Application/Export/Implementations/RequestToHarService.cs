using System;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Cookie = HttPlaceholder.Application.StubExecution.Models.HAR.Cookie;
using ConvertUtil = System.Convert;

namespace HttPlaceholder.Application.Export.Implementations;

internal class RequestToHarService : IRequestToHarService, ISingletonService
{
    private readonly IAssemblyService _assemblyService;

    public RequestToHarService(IAssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    public string Convert(RequestResultModel request, ResponseModel response)
    {
        var requestParams = request.RequestParameters;
        var requestContentType = requestParams.Headers.FirstOrDefault(h =>
            h.Key.Equals(HeaderKeys.ContentType, StringComparison.OrdinalIgnoreCase)).Value;
        var responseContentType = response.Headers.FirstOrDefault(h =>
            h.Key.Equals(HeaderKeys.ContentType, StringComparison.OrdinalIgnoreCase)).Value;
        var responseBody = response.Body ?? Array.Empty<byte>();
        var uri = new Uri(requestParams.Url);
        var query = QueryHelpers.ParseQuery(uri.Query);
        const string creator = "HttPlaceholder";
        const string httpVersion = "HTTP/1.1";
        var pageId = $"page_{request.CorrelationId}";
        var version = _assemblyService.GetAssemblyVersion();
        var har = new Har
        {
            Log = new Log
            {
                Version = "1.2",
                Creator = new Creator {Name = creator, Version = version},
                Pages =
                    new[]
                    {
                        new Page
                        {
                            Id = pageId,
                            StartedDateTime = request.RequestBeginTime,
                            Title = string.Empty,
                            PageTimings = new PageTimings()
                        }
                    },
                Entries = new[]
                {
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
                                    .Select(h => new Header {Name = h.Key, Value = h.Value}).ToArray(),
                            Cookies = Array.Empty<Cookie>(),
                            PostData = new PostData
                            {
                                MimeType = requestContentType,
                                Text =
                                    Encoding.UTF8.GetString(requestParams.BinaryBody ??
                                                            Array.Empty<byte>())
                            },
                            QueryString = query.Select(q => new Query
                            {
                                Name = q.Key, Value = q.Value.FirstOrDefault()
                            }).ToArray()
                        },
                        Response = new Response
                        {
                            Status = response.StatusCode,
                            StatusText = ReasonPhrases.GetReasonPhrase(response.StatusCode),
                            HttpVersion = httpVersion,
                            Headers = response.Headers.Select(h => new Header
                            {
                                Name = h.Key,
                                Value = h.Value
                            }).ToArray(),
                            Cookies = Array.Empty<Cookie>(),
                            Content = new Content
                            {
                                Encoding = response.BodyIsBinary ? "base64" : null,
                                MimeType = responseContentType,
                                Size = responseBody.Length,
                                Text = response.BodyIsBinary ? ConvertUtil.ToBase64String(responseBody) : Encoding.UTF8.GetString(responseBody)
                            },
                            RedirectUrl = string.Empty
                        },
                        Cache = new Cache(),
                        Timings = new Timings(),
                        Time = 0,
                        ServerIpAddress = string.Empty,
                        Connection = string.Empty
                    }
                }
            }
        };
        return JsonConvert.SerializeObject(har);
    }
}
