using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// Handler that is being used for setting the response body.
/// </summary>
internal class ResponseBodyHandler : IResponseToStubResponseHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel)
    {
        if (response.ContentIsBase64)
        {
            stubResponseModel.Base64 = response.Content;
            return Task.FromResult(true);
        }

        var contentType = string.IsNullOrWhiteSpace(stubResponseModel.ContentType)
            ? null
            : stubResponseModel.ContentType.Split(';')[0].ToLower();
        switch (contentType)
        {
            case Constants.HtmlMime:
                stubResponseModel.Html = response.Content;
                break;
            case Constants.JsonMime:
                stubResponseModel.Json = response.Content;
                break;
            case Constants.XmlTextMime:
            case Constants.XmlApplicationMime:
                stubResponseModel.Xml = response.Content;
                break;
            default:
                stubResponseModel.Text = response.Content;
                break;
        }

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => -2;
}
