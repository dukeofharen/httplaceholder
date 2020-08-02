using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Configuration;
using HttPlaceholder.Dto.v1.Requests;
using HttPlaceholder.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StubHandlingMiddleware
    {
        private static readonly string[] _segmentsToIgnore = {"/ph-api", "/ph-ui", "swagger", "/requestHub"};

        private readonly RequestDelegate _next;
        private readonly IClientDataResolver _clientDataResolver;
        private readonly IHttpContextService _httpContextService;
        private readonly ILogger<StubHandlingMiddleware> _logger;
        private readonly IRequestLoggerFactory _requestLoggerFactory;
        private readonly IStubContext _stubContext;
        private readonly IStubRequestExecutor _stubRequestExecutor;
        private readonly IRequestNotify _requestNotify;
        private readonly SettingsModel _settings;
        private readonly IMapper _mapper;

        public StubHandlingMiddleware(
            RequestDelegate next,
            IClientDataResolver clientDataResolver,
            IHttpContextService httpContextService,
            ILogger<StubHandlingMiddleware> logger,
            IRequestLoggerFactory requestLoggerFactory,
            IRequestNotify requestNotify,
            IStubContext stubContext,
            IStubRequestExecutor stubRequestExecutor,
            IOptions<SettingsModel> options,
            IMapper mapper)
        {
            _next = next;
            _clientDataResolver = clientDataResolver;
            _httpContextService = httpContextService;
            _logger = logger;
            _requestLoggerFactory = requestLoggerFactory;
            _requestNotify = requestNotify;
            _stubContext = stubContext;
            _stubRequestExecutor = stubRequestExecutor;
            _mapper = mapper;
            _settings = options.Value;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            var path = _httpContextService.Path;
            if (_segmentsToIgnore.Any(s => path.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            const string correlationHeaderKey = "X-HttPlaceholder-Correlation";
            var correlation = Guid.NewGuid().ToString();
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
                    _clientDataResolver.GetClientIp(),
                    _httpContextService.GetHeaders());

                _httpContextService.ClearResponse();
                _httpContextService.TryAddHeader(correlationHeaderKey, correlation);
                var response = await _stubRequestExecutor.ExecuteRequestAsync();
                _httpContextService.SetStatusCode(response.StatusCode);
                foreach (var (key, value) in response.Headers)
                {
                    _httpContextService.AddHeader(key, value);
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
                _logger.LogInformation($"Request validation exception thrown: {e.Message}");
            }
            catch (Exception e)
            {
                _httpContextService.SetStatusCode((int)HttpStatusCode.InternalServerError);
                _httpContextService.TryAddHeader(correlationHeaderKey, correlation);
                _logger.LogWarning($"Unexpected exception thrown: {e}");
            }

            var loggingResult = requestLogger.GetResult();
            var jsonLoggingResult = JObject.FromObject(loggingResult);
            var enableRequestLogging = _settings.Storage?.EnableRequestLogging ?? false;
            if (enableRequestLogging)
            {
                _logger.LogInformation(jsonLoggingResult.ToString());
            }

            await _stubContext.AddRequestResultAsync(loggingResult);

            // We need to map the model to a DTO here, because the frontend expected that.
            await _requestNotify.NewRequestReceivedAsync(_mapper.Map<RequestResultDto>(loggingResult));
        }
    }
}
