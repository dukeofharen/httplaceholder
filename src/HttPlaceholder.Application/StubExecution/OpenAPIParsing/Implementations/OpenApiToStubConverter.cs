using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

/// <inheritdoc />
public class OpenApiToStubConverter : IOpenApiToStubConverter
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
    public async Task<StubModel> ConvertToStubAsync(string serverUrl, OpenApiLine line)
    {
        var request = new HttpRequestModel
        {
            Body = _openApiDataFiller.BuildRequestBody(line.Operation),
            Headers = _openApiDataFiller.BuildRequestHeaders(line.Operation),
            Method = line.OperationType.ToString().ToUpper(),
            Url = $"{serverUrl}{_openApiDataFiller.BuildRelativeRequestPath(line.Operation, line.PathKey)}"
        };
        var response = new HttpResponseModel
        {
            Content = _openApiDataFiller.BuildResponseBody(line.Response),
            Headers = _openApiDataFiller.BuildResponseHeaders(line.Response),
            StatusCode = _openApiDataFiller.BuildHttpStatusCode(line.ResponseKey)
        };
        var stub = new StubModel
        {
            Description = line.Operation.Summary,
            Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request),
            Response = await _httpResponseToStubResponseService.ConvertToResponseAsync(response),
        };
        stub.Id = $"generated-{HashingUtilities.GetMd5String(JsonConvert.SerializeObject(stub))}";
        return stub;
    }
}
