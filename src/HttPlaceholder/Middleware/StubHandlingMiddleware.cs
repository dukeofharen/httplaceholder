using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ducode.Essentials.Console;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Exceptions;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Middleware
{
    public class StubHandlingMiddleware
    {
        private static string[] _segmentsToIgnore = new[]
        {
         "/ph-api",
         "/ph-ui",
         "swagger"
      };

        private readonly RequestDelegate _next;
        private readonly IClientIpResolver _clientIpResolver;
        private readonly IConfigurationService _configurationService;
        private readonly IHttpContextService _httpContextService;
        private readonly ILogger<StubHandlingMiddleware> _logger;
        private readonly IRequestLoggerFactory _requestLoggerFactory;
        private readonly IStubContainer _stubContainer;
        private readonly IStubRequestExecutor _stubRequestExecutor;

        public StubHandlingMiddleware(
           RequestDelegate next,
           IClientIpResolver clientIpResolver,
           IConfigurationService configurationService,
           IHttpContextService httpContextService,
           ILogger<StubHandlingMiddleware> logger,
           IRequestLoggerFactory requestLoggerFactory,
           IStubContainer stubContainer,
           IStubRequestExecutor stubRequestExecutor)
        {
            _next = next;
            _clientIpResolver = clientIpResolver;
            _configurationService = configurationService;
            _httpContextService = httpContextService;
            _logger = logger;
            _requestLoggerFactory = requestLoggerFactory;
            _stubContainer = stubContainer;
            _stubRequestExecutor = stubRequestExecutor;
        }

        public async Task Invoke(HttpContext context)
        {
            string path = _httpContextService.Path;
            if (_segmentsToIgnore.Any(s => path.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            const string correlationHeaderKey = "X-HttPlaceholder-Correlation";
            string correlation = Guid.NewGuid().ToString();
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            requestLogger.SetCorrelationId(correlation);
            try
            {
                // Enable rewind here to be able to read the posted body multiple times.
                _httpContextService.EnableRewind();

                // Log the request here
                requestLogger.LogRequestParameters(
                   _httpContextService.Method,
                   _httpContextService.DisplayUrl,
                   _httpContextService.GetBody(),
                   _clientIpResolver.GetClientIp(),
                   _httpContextService.GetHeaders());

                _httpContextService.ClearResponse();
                _httpContextService.TryAddHeader(correlationHeaderKey, correlation);
                var response = await _stubRequestExecutor.ExecuteRequestAsync();
                _httpContextService.SetStatusCode(response.StatusCode);
                foreach (var header in response.Headers)
                {
                    _httpContextService.AddHeader(header.Key, header.Value);
                }

                if (response.Body != null)
                {
                    await _httpContextService.WriteAsync(response.Body);
                }
            }
            catch (RequestValidationException e)
            {
                _httpContextService.SetStatusCode((int)HttpStatusCode.InternalServerError);
                _httpContextService.TryAddHeader(correlationHeaderKey, correlation);
                _logger.LogWarning($"Request validation exception thrown: {e.Message}");
            }
            catch (Exception e)
            {
                _httpContextService.SetStatusCode((int)HttpStatusCode.InternalServerError);
                _httpContextService.TryAddHeader(correlationHeaderKey, correlation);
                _logger.LogWarning($"Unexpected exception thrown: {e}");
            }

            var loggingResult = requestLogger.GetResult();
            var jsonLoggingResult = JObject.FromObject(loggingResult);
            var config = _configurationService.GetConfiguration();
            bool enableRequestLogging = config.GetValue(Constants.ConfigKeys.EnableRequestLogging, true);
            if (enableRequestLogging)
            {
                _logger.LogInformation(jsonLoggingResult.ToString());
            }

            await _stubContainer.AddRequestResultAsync(loggingResult);
        }
    }
}