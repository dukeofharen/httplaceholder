﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc />
    internal class CurlStubGenerator : ICurlStubGenerator
    {
        private readonly ICurlToHttpRequestMapper _curlToHttpRequestMapper;
        private readonly ILogger<CurlStubGenerator> _logger;
        private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
        private readonly IStubContext _stubContext;

        public CurlStubGenerator(
            ICurlToHttpRequestMapper curlToHttpRequestMapper,
            ILogger<CurlStubGenerator> logger,
            IHttpRequestToConditionsService httpRequestToConditionsService,
            IStubContext stubContext)
        {
            _curlToHttpRequestMapper = curlToHttpRequestMapper;
            _logger = logger;
            _httpRequestToConditionsService = httpRequestToConditionsService;
            _stubContext = stubContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubModel>> GenerateCurlStubsAsync(string input, bool doNotCreateStub)
        {
            _logger.LogDebug($"Creating stubs based on cURL command {input}.");
            var requests = _curlToHttpRequestMapper.MapCurlCommandsToHttpRequest(input);
            var results = new List<FullStubModel>();
            foreach (var request in requests)
            {
                var stub = new StubModel
                {
                    Conditions = await _httpRequestToConditionsService.ConvertToConditionsAsync(request),
                    Response = { Text = "OK!" }
                };

                // Generate an ID based on the created stub.
                var contents = JsonConvert.SerializeObject(stub);
                stub.Id = "generated-" + HashingUtilities.GetMd5String(contents);
                results.Add(await CreateStub(doNotCreateStub, stub));
            }

            return results;
        }

        private async Task<FullStubModel> CreateStub(bool doNotCreateStub, StubModel stub)
        {
            if (doNotCreateStub)
            {
                return new FullStubModel { Stub = stub, Metadata = new StubMetadataModel() };
            }

            await _stubContext.DeleteStubAsync(stub.Id);
            return await _stubContext.AddStubAsync(stub);
        }
    }
}
