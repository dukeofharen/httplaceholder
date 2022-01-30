using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class HarStubGenerator : IHarStubGenerator
{
    private static string[] _responseHeadersToStrip = new[] {"content-length", "content-encoding"};

    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly IHttpResponseToStubResponseService _httpResponseToStubResponseService;
    private readonly IStubContext _stubContext;

    public HarStubGenerator(
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IHttpResponseToStubResponseService httpResponseToStubResponseService,
        IStubContext stubContext)
    {
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _httpResponseToStubResponseService = httpResponseToStubResponseService;
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateHarStubsAsync(string input, bool doNotCreateStub,
        string tenant)
    {
        try
        {
            var har = JsonConvert.DeserializeObject<Har>(input);
            ValidateHar(har);
            var stubs = har.Log.Entries
                .Select(e => (req: MapRequest(e), res: MapResponse(e)))
                .Select(t => MapStub(t.req, t.res, tenant))
                .Select(r => r.Result);
            var result = new List<FullStubModel>();
            foreach (var stub in stubs)
            {
                result.Add(await CreateStub(doNotCreateStub, stub));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Exception occurred while trying to parse HTTP Archive: {ex.Message}");
        }
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, StubModel stub)
    {
        if (doNotCreateStub)
        {
            return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }

        await _stubContext.DeleteStubAsync(stub.Id);
        return await _stubContext.AddStubAsync(stub);
    }

    private static HttpRequestModel MapRequest(Entry entry) => new()
    {
        Method = entry.Request.Method,
        Body = entry.Request.PostData?.Text,
        Headers = entry.Request.Headers
            .Where(h => !h.Name.StartsWith(":"))
            .ToDictionary(h => h.Name, h => h.Value),
        Url = entry.Request.Url
    };

    private static HttpResponseModel MapResponse(Entry entry) => new()
    {
        StatusCode = entry.Response.Status,
        Content = entry.Response.Content?.Text,
        ContentIsBase64 =
            string.Equals(entry.Response.Content?.Encoding, "base64", StringComparison.OrdinalIgnoreCase),
        Headers = entry.Response.Headers
            .Where(h => !_responseHeadersToStrip.Contains(h.Name.ToLower()))
            .ToDictionary(h => h.Name, h => h.Value)
    };

    private async Task<StubModel> MapStub(HttpRequestModel req, HttpResponseModel res, string tenant)
    {
        var conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(req);
        var response = await _httpResponseToStubResponseService.ConvertToResponseAsync(res);
        var stub = new StubModel
        {
            Tenant = tenant,
            Description = $"{conditions.Method} request to path {conditions.Url?.Path}",
            Conditions = conditions,
            Response = response
        };
        stub.EnsureStubId();
        return stub;
    }

    private static void ValidateHar(Har har)
    {
        if (har == null)
        {
            throw new ValidationException("The HAR was invalid.");
        }

        if (har.Log == null)
        {
            throw new ValidationException("har.log is not set.");
        }

        if (har.Log.Entries == null || !har.Log.Entries.Any())
        {
            throw new ValidationException("No entries set in HAR.");
        }
    }
}
