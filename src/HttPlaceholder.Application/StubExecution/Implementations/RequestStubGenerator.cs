using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc />
    internal class RequestStubGenerator : IRequestStubGenerator
    {
        private readonly ILogger<RequestStubGenerator> _logger;
        private readonly IStubContext _stubContext;
        private readonly IMapper _mapper;
        private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;

        public RequestStubGenerator(
            IStubContext stubContext,
            ILogger<RequestStubGenerator> logger,
            IMapper mapper,
            IHttpRequestToConditionsService httpRequestToConditionsService)
        {
            _stubContext = stubContext;
            _logger = logger;
            _mapper = mapper;
            _httpRequestToConditionsService = httpRequestToConditionsService;
        }

        /// <inheritdoc />
        public async Task<FullStubModel> GenerateStubBasedOnRequestAsync(
            string requestCorrelationId,
            bool doNotCreateStub)
        {
            _logger.LogDebug($"Creating stub based on request with corr.ID '{requestCorrelationId}'.");
            var requestResult = await _stubContext.GetRequestResultAsync(requestCorrelationId);
            if (requestResult == null)
            {
                throw new NotFoundException(nameof(RequestResultModel), requestCorrelationId);
            }

            var request = _mapper.Map<HttpRequestModel>(requestResult.RequestParameters);
            var stub = new StubModel
            {
                Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request),
                Response = { Text = "OK!" }
            };

            // Generate an ID based on the created stub.
            var contents = JsonConvert.SerializeObject(stub);
            stub.Id = "generated-" + HashingUtilities.GetMd5String(contents);

            FullStubModel result;
            if (doNotCreateStub)
            {
                result = new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
            }
            else
            {
                await _stubContext.DeleteStubAsync(stub.Id);
                result = await _stubContext.AddStubAsync(stub);
            }

            _logger.LogInformation($"Stub with ID '{stub.Id}' generated!");

            return result;
        }
    }
}
