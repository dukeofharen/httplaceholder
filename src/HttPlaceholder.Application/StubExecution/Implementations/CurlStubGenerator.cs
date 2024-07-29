using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class CurlStubGenerator(
    ICurlToHttpRequestMapper curlToHttpRequestMapper,
    ILogger<CurlStubGenerator> logger,
    IHttpRequestToConditionsService httpRequestToConditionsService,
    IStubContext stubContext)
    : ICurlStubGenerator, ISingletonService
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateStubsAsync(string input, bool doNotCreateStub, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating stubs based on cURL command {CurlCommand}.", input);
        var requests = curlToHttpRequestMapper.MapCurlCommandsToHttpRequest(input);
        var results = new List<FullStubModel>();
        foreach (var request in requests)
        {
            var conditions = await httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken);
            var stub = new StubModel
            {
                Tenant = tenant,
                Description = string.Format(StubResources.MethodStubDescription, conditions.Method, conditions.Url?.Path),
                Conditions = conditions,
                Response = { Text = "OK!" }
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
            return new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
        }

        await stubContext.DeleteStubAsync(stub.Id, cancellationToken);
        return await stubContext.AddStubAsync(stub, cancellationToken);
    }
}
