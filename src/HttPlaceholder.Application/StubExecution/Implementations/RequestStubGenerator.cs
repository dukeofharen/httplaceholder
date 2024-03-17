using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class RequestStubGenerator(
    IStubContext stubContext,
    ILogger<RequestStubGenerator> logger,
    IMapper mapper,
    IHttpRequestToConditionsService httpRequestToConditionsService,
    IHttpResponseToStubResponseService httpResponseToStubResponseService)
    : IRequestStubGenerator, ISingletonService
{
    /// <inheritdoc />
    public async Task<FullStubModel> GenerateStubBasedOnRequestAsync(
        string requestCorrelationId,
        bool doNotCreateStub,
        CancellationToken cancellationToken)
    {
        logger.LogDebug($"Creating stub based on request with corr.ID '{requestCorrelationId}'.");
        var requestResult = await stubContext.GetRequestResultAsync(requestCorrelationId, cancellationToken)
            .IfNullAsync(() => throw new NotFoundException(nameof(RequestResultModel), requestCorrelationId));
        var request = mapper.Map<HttpRequestModel>(requestResult.RequestParameters);
        var response = await stubContext.GetResponseAsync(requestCorrelationId, cancellationToken);
        var httpResponseModel = mapper.Map<HttpResponseModel>(response);
        var stubResponse = httpResponseModel != null
            ? await httpResponseToStubResponseService.ConvertToResponseAsync(httpResponseModel, cancellationToken)
            : new StubResponseModel { Text = "OK!" };
        var stub = new StubModel
        {
            Conditions = await httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken),
            Response = stubResponse
        };

        // Generate an ID based on the created stub.
        stub.EnsureStubId();

        FullStubModel result;
        if (doNotCreateStub)
        {
            result = new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
        }
        else
        {
            await stubContext.DeleteStubAsync(stub.Id, cancellationToken);
            result = await stubContext.AddStubAsync(stub, cancellationToken);
        }

        logger.LogDebug($"Stub with ID '{stub.Id}' generated!");

        return result;
    }
}
