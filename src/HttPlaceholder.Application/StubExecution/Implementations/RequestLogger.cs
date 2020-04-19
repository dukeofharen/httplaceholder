using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    internal class RequestLogger : IRequestLogger
    {
        private readonly IDateTime _dateTime;
        private readonly RequestResultModel _result;

        public RequestLogger(IDateTime dateTime)
        {
            _dateTime = dateTime;
            _result = new RequestResultModel
            {
                RequestBeginTime = _dateTime.UtcNow
            };
        }

        public RequestResultModel GetResult()
        {
            _result.RequestEndTime = _dateTime.UtcNow;
            return _result;
        }

        public void LogRequestParameters(string method, string url, string body, string clientIp, IDictionary<string, string> headers) =>
            _result.RequestParameters = new RequestParametersModel
            {
                Body = body,
                ClientIp = clientIp,
                Headers = headers,
                Method = method,
                Url = url
            };

        public void SetCorrelationId(string correlationId) => _result.CorrelationId = correlationId;

        public void SetStubExecutionResult(string stubId, bool passed, IEnumerable<ConditionCheckResultModel> conditions, IEnumerable<ConditionCheckResultModel> negativeConditions)
        {
            // Do not log the conditions with validation type "NotExecuted". They severely pollute the logging and API.
            conditions = conditions.Where(m => m.ConditionValidation != ConditionValidationType.NotExecuted);
            negativeConditions = negativeConditions.Where(m => m.ConditionValidation != ConditionValidationType.NotExecuted);
            _result.StubExecutionResults.Add(new StubExecutionResultModel
            {
                Passed = passed,
                StubId = stubId,
                Conditions = conditions,
                NegativeConditions = negativeConditions
            });
        }

        public void SetExecutingStubId(string stubId) => _result.ExecutingStubId = stubId;

        public void SetResponseWriterResult(string writerName, bool executed) =>
            _result.StubResponseWriterResults.Add(new StubResponseWriterResultModel
            {
                Executed = executed,
                ResponseWriterName = writerName
            });
    }
}
