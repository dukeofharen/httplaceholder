using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using HttPlaceholder.Exceptions;
using HttPlaceholder.BusinessLogic;
using Newtonsoft.Json.Linq;
using HttPlaceholder.Models;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.Middleware
{
   public class StubHandlingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly IConfigurationService _configurationService;
      private readonly IHttpContextService _httpContextService;
      private readonly ILogger<StubHandlingMiddleware> _logger;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IStubContainer _stubContainer;
      private readonly IStubRequestExecutor _stubRequestExecutor;

      public StubHandlingMiddleware(
         RequestDelegate next,
         IConfigurationService configurationService,
         IHttpContextService httpContextService,
         ILogger<StubHandlingMiddleware> logger,
         IRequestLoggerFactory requestLoggerFactory,
         IStubContainer stubContainer,
         IStubRequestExecutor stubRequestExecutor)
      {
         _next = next;
         _configurationService = configurationService;
         _httpContextService = httpContextService;
         _logger = logger;
         _requestLoggerFactory = requestLoggerFactory;
         _stubContainer = stubContainer;
         _stubRequestExecutor = stubRequestExecutor;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context.Request.Path.StartsWithSegments("/ph-api") || context.Request.Path.Value.Contains("swagger"))
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
            context.Request.EnableRewind();

            // Log the request here
            requestLogger.LogRequestParameters(
               _httpContextService.Method,
               _httpContextService.DisplayUrl,
               _httpContextService.GetBody(),
               _httpContextService.GetClientIp(),
               _httpContextService.GetHeaders());

            context.Response.Clear();
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
            var response = await _stubRequestExecutor.ExecuteRequestAsync();
            context.Response.StatusCode = response.StatusCode;
            foreach (var header in response.Headers)
            {
               context.Response.Headers.Add(header.Key, header.Value);
            }

            if (response.Body != null)
            {
               await context.Response.Body.WriteAsync(response.Body, 0, response.Body.Length);
            }
         }
         catch (RequestValidationException e)
         {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
            _logger.LogWarning($"Request validation exception thrown: {e.Message}");
         }
         catch (Exception e)
         {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
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
