using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class RequestLoggerFactory : IRequestLoggerFactory
{
    private readonly IDateTime _dateTime;
    private readonly IHttpContextService _httpContextService;

    public RequestLoggerFactory(
        IDateTime dateTime,
        IHttpContextService httpContextService)
    {
        _dateTime = dateTime;
        _httpContextService = httpContextService;
    }

    public IRequestLogger GetRequestLogger()
    {
        const string requestLoggerKey = "requestLogger";
        var requestLogger = _httpContextService.GetItem<IRequestLogger>(requestLoggerKey);
        if (requestLogger != null)
        {
            return requestLogger;
        }

        requestLogger = new RequestLogger(_dateTime);
        _httpContextService.SetItem(requestLoggerKey, requestLogger);
        return requestLogger;
    }
}