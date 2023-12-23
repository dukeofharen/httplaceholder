using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

internal class OpenApiToStubConverter(
    IHttpRequestToConditionsService httpRequestToConditionsService,
    IHttpResponseToStubResponseService httpResponseToStubResponseService,
    IOpenApiDataFiller openApiDataFiller)
    : IOpenApiToStubConverter, ISingletonService
{
    /// <inheritdoc />
    public async Task<StubModel> ConvertToStubAsync(OpenApiServer server, OpenApiLine line, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestModel
        {
            Body = openApiDataFiller.BuildRequestBody(line.Operation),
            Headers = openApiDataFiller.BuildRequestHeaders(line.Operation),
            Method = line.OperationType.ToString().ToUpper(),
            Url =
                $"{openApiDataFiller.BuildServerUrl(server)}{openApiDataFiller.BuildRelativeRequestPath(line.Operation, line.PathKey)}"
        };
        var response = new HttpResponseModel
        {
            Content = openApiDataFiller.BuildResponseBody(line.Response),
            Headers = openApiDataFiller.BuildResponseHeaders(line.Response),
            StatusCode = openApiDataFiller.BuildHttpStatusCode(line.ResponseKey)
        };
        var stub = new StubModel
        {
            Tenant = tenant,
            Description = line.Operation.Summary,
            Conditions = await httpRequestToConditionsService.ConvertToConditionsAsync(request, cancellationToken),
            Response = await httpResponseToStubResponseService.ConvertToResponseAsync(response, cancellationToken)
        };
        stub.EnsureStubId(stubIdPrefix);
        return stub;
    }
}
