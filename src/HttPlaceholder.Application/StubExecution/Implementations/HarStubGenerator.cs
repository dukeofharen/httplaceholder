using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class HarStubGenerator(
    IHttpRequestToConditionsService httpRequestToConditionsService,
    IHttpResponseToStubResponseService httpResponseToStubResponseService,
    IStubContext stubContext)
    : IHarStubGenerator, ISingletonService
{
    private static readonly string[] _responseHeadersToStrip = { "content-length", "content-encoding" };

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateStubsAsync(string input, bool doNotCreateStub, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        try
        {
            var har = JsonConvert.DeserializeObject<Har>(input);
            ValidateHar(har);
            var stubs = await Task.WhenAll(har.Log.Entries
                .Select(e => (req: MapRequest(e), res: MapResponse(e)))
                .Select(t => MapStub(t.req, t.res, tenant, stubIdPrefix, cancellationToken)));
            var result = new List<FullStubModel>();
            foreach (var stub in stubs)
            {
                result.Add(await CreateStub(doNotCreateStub, stub, cancellationToken));
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Exception occurred while trying to parse HTTP Archive: {ex.Message}");
        }
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, StubModel stub,
        CancellationToken cancellationToken)
    {
        if (doNotCreateStub)
        {
            return new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
        }

        await stubContext.DeleteStubAsync(stub.Id, cancellationToken);
        return await stubContext.AddStubAsync(stub, cancellationToken);
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

    private async Task<StubModel> MapStub(HttpRequestModel req, HttpResponseModel res, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        var conditions = await httpRequestToConditionsService.ConvertToConditionsAsync(req, cancellationToken);
        var response = await httpResponseToStubResponseService.ConvertToResponseAsync(res, cancellationToken);
        var stub = new StubModel
        {
            Tenant = tenant,
            Description = $"{conditions.Method} request to path {conditions.Url?.Path}",
            Conditions = conditions,
            Response = response
        };
        stub.EnsureStubId(stubIdPrefix);
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
