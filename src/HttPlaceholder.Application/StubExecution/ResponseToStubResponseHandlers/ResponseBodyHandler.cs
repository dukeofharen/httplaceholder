using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
///     Handler that is being used for setting the response body.
/// </summary>
internal class ResponseBodyHandler : IResponseToStubResponseHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel,
        CancellationToken cancellationToken)
    {
        if (response.ContentIsBase64)
        {
            stubResponseModel.Base64 = response.Content;
            return true.AsTask();
        }

        var contentType = string.IsNullOrWhiteSpace(stubResponseModel.ContentType)
            ? null
            : stubResponseModel.ContentType.Split(';')[0].ToLower();
        switch (contentType)
        {
            case MimeTypes.HtmlMime:
                stubResponseModel.Html = response.Content;
                break;
            case MimeTypes.JsonMime:
                stubResponseModel.Json = response.Content;
                break;
            case MimeTypes.XmlTextMime:
            case MimeTypes.XmlApplicationMime:
                stubResponseModel.Xml = response.Content;
                break;
            default:
                stubResponseModel.Text = response.Content;
                break;
        }

        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => -2;
}
