using Ducode.Essentials.Mvc.Interfaces;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    internal class RequestLoggerFactory : IRequestLoggerFactory
    {
        private readonly IHttpContextService _httpContextService;

        public RequestLoggerFactory(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public IRequestLogger GetRequestLogger()
        {
            const string requestLoggerKey = "requestLogger";
            var requestLogger = _httpContextService.GetItem<IRequestLogger>(requestLoggerKey);
            if (requestLogger == null)
            {
                requestLogger = new RequestLogger();
                _httpContextService.SetItem(requestLoggerKey, requestLogger);
            }

            return requestLogger;
        }
    }
}
