using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class HarStubGenerator : IHarStubGenerator
{
    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly IStubContext _stubContext;

    public HarStubGenerator(
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IStubContext stubContext)
    {
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateHarStubsAsync(string input, bool doNotCreateStub)
    {
        try
        {
            var har = JsonConvert.DeserializeObject<Har>(input);
            ValidateHar(har);
            var stubs = har.Log.Entries
                .Select(e => new HttpRequestModel
                {
                    Method = e.Request.Method,
                    Body = e.Request.PostData?.Text,
                    Headers = e.Request.Headers
                        .Where(h => !h.Name.StartsWith(":"))
                        .ToDictionary(h => h.Name, h => h.Value),
                    Url = e.Request.Url
                })
                .Select(async r =>
                {
                    var stub = new StubModel
                    {
                        Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(r),
                        Response = {Text = "OK!"} // TODO
                    };
                    stub.Id = GenerateId(stub);
                    return stub;
                })
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

    private string GenerateId(StubModel stub) => $"generated-{HashingUtilities.GetMd5String(JsonConvert.SerializeObject(stub))}";

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, StubModel stub)
    {
        if (doNotCreateStub)
        {
            return new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
        }

        await _stubContext.DeleteStubAsync(stub.Id);
        return await _stubContext.AddStubAsync(stub);
    }

    private void ValidateHar(Har har)
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
