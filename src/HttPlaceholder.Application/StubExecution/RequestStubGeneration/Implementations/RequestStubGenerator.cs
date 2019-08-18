using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class RequestStubGenerator : IRequestStubGenerator
    {
        private readonly ILogger<RequestStubGenerator> _logger;
        private readonly IStubContext _stubContext;
        private readonly IEnumerable<IRequestStubGenerationHandler> _handlers;

        public RequestStubGenerator(
            IStubContext stubContext,
            IEnumerable<IRequestStubGenerationHandler> handlers,
            ILogger<RequestStubGenerator> logger)
        {
            _stubContext = stubContext;
            _handlers = handlers;
            _logger = logger;
        }

        public async Task<StubModel> GenerateStubBasedOnRequestAsync(string requestCorrelationId)
        {
            _logger.LogInformation($"Creating stub based on request with corr.ID '{requestCorrelationId}'.");

            // TODO lateron, when the querying is fixed, only query for one request result.
            var requestResults = await _stubContext.GetRequestResultsAsync();
            var requestResult = requestResults.FirstOrDefault(r => r.CorrelationId == requestCorrelationId);
            if (requestResult == null)
            {
                throw new NotFoundException(nameof(RequestResultModel), requestCorrelationId);
            }

            var stub = new StubModel();
            foreach (var handler in _handlers.OrderByDescending(w => w.Priority))
            {
                bool executed = await handler.HandleStubGenerationAsync(requestResult, stub);
                _logger.LogInformation($"Handler '{handler.GetType().Name}'" + (executed ? " executed" : "") + ".");
            }

            // Generate an ID based on the created stub.
            string contents = JsonConvert.SerializeObject(stub);
            stub.Id = "generated-" + HashingUtilities.GetMd5String(contents);
            await _stubContext.DeleteStubAsync(stub.Id);
            await _stubContext.AddStubAsync(stub);
            _logger.LogInformation($"Stub with ID '{stub.Id}' generated!");

            return stub;
        }
    }
}
