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

        public async Task<FullStubModel> GenerateStubBasedOnRequestAsync(string requestCorrelationId,
            bool doNotCreateStub)
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
                var executed = await handler.HandleStubGenerationAsync(requestResult, stub);
                _logger.LogInformation($"Handler '{handler.GetType().Name}'" + (executed ? " executed" : "") + ".");
            }

            // Set a default response
            stub.Response.Text = "OK!";

            // Generate an ID based on the created stub.
            var contents = JsonConvert.SerializeObject(stub);
            stub.Id = "generated-" + HashingUtilities.GetMd5String(contents);

            FullStubModel result;
            if (doNotCreateStub)
            {
                result = new FullStubModel {Stub = stub, Metadata = new StubMetadataModel()};
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
