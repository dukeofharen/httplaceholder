using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class CurlStubGenerator : ICurlStubGenerator, ISingletonService
{
    private readonly ICurlToHttpRequestMapper _curlToHttpRequestMapper;
    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly ILogger<CurlStubGenerator> _logger;
    private readonly IStubContext _stubContext;

    public CurlStubGenerator(
        ICurlToHttpRequestMapper curlToHttpRequestMapper,
        ILogger<CurlStubGenerator> logger,
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IStubContext stubContext)
    {
        _curlToHttpRequestMapper = curlToHttpRequestMapper;
        _logger = logger;
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateStubsAsync(string input, bool doNotCreateStub, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Creating stubs based on cURL command {input}.");
        var requests = _curlToHttpRequestMapper.MapCurlCommandsToHttpRequest(input);
        var results = new List<FullStubModel>();
        foreach (var request in requests)
        {
            var conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken);
            var stub = new StubModel
            {
                Tenant = tenant,
                Description = $"{conditions.Method} request to path {conditions.Url?.Path}",
                Conditions = conditions,
                Response = {Text = "OK!"}
            };

            // Generate an ID based on the created stub.
            stub.EnsureStubId(stubIdPrefix);
            results.Add(await CreateStub(doNotCreateStub, stub, cancellationToken));
        }

        return results;
    }

    private async Task<FullStubModel> CreateStub(bool doNotCreateStub, StubModel stub,
        CancellationToken cancellationToken)
    {
        if (doNotCreateStub)
        {
            return new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
        }

        await _stubContext.DeleteStubAsync(stub.Id, cancellationToken);
        return await _stubContext.AddStubAsync(stub, cancellationToken);
    }
}
