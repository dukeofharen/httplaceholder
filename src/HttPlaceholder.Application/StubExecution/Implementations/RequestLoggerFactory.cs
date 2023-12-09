using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class RequestLoggerFactory(
    IDateTime dateTime,
    IHttpContextService httpContextService)
    : IRequestLoggerFactory, ISingletonService
{
    /// <inheritdoc />
    public IRequestLogger GetRequestLogger()
    {
        const string requestLoggerKey = "requestLogger";
        var requestLogger = httpContextService.GetItem<IRequestLogger>(requestLoggerKey);
        if (requestLogger != null)
        {
            return requestLogger;
        }

        requestLogger = new RequestLogger(dateTime);
        httpContextService.SetItem(requestLoggerKey, requestLogger);
        return requestLogger;
    }
}
