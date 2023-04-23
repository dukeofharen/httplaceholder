using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

internal class OpenApiToStubConverter : IOpenApiToStubConverter, ISingletonService
{
    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly IHttpResponseToStubResponseService _httpResponseToStubResponseService;
    private readonly IOpenApiDataFiller _openApiDataFiller;

    public OpenApiToStubConverter(
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IHttpResponseToStubResponseService httpResponseToStubResponseService,
        IOpenApiDataFiller openApiDataFiller)
    {
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _httpResponseToStubResponseService = httpResponseToStubResponseService;
        _openApiDataFiller = openApiDataFiller;
    }

    /// <inheritdoc />
    public async Task<StubModel> ConvertToStubAsync(OpenApiServer server, OpenApiLine line, string tenant, string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestModel
        {
            Body = _openApiDataFiller.BuildRequestBody(line.Operation),
            Headers = _openApiDataFiller.BuildRequestHeaders(line.Operation),
            Method = line.OperationType.ToString().ToUpper(),
            Url =
                $"{_openApiDataFiller.BuildServerUrl(server)}{_openApiDataFiller.BuildRelativeRequestPath(line.Operation, line.PathKey)}"
        };
        var response = new HttpResponseModel
        {
            Content = _openApiDataFiller.BuildResponseBody(line.Response),
            Headers = _openApiDataFiller.BuildResponseHeaders(line.Response),
            StatusCode = _openApiDataFiller.BuildHttpStatusCode(line.ResponseKey)
        };
        var stub = new StubModel
        {
            Tenant = tenant,
            Description = line.Operation.Summary,
            Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken),
            Response = await _httpResponseToStubResponseService.ConvertToResponseAsync(response, cancellationToken)
        };
        stub.EnsureStubId(stubIdPrefix);
        return stub;
    }
}
