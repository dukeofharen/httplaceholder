using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class RequestLogger : IRequestLogger
{
    private readonly IDateTime _dateTime;
    private readonly RequestResultModel _result;

    public RequestLogger(IDateTime dateTime)
    {
        _dateTime = dateTime;
        _result = new RequestResultModel { RequestBeginTime = _dateTime.UtcNow };
    }

    /// <inheritdoc />
    public RequestResultModel GetResult()
    {
        _result.RequestEndTime = _dateTime.UtcNow;
        return _result;
    }

    /// <inheritdoc />
    public void LogRequestParameters(string method, string url, byte[] body, string clientIp,
        IDictionary<string, string> headers) =>
        _result.RequestParameters = new RequestParametersModel
        {
            BinaryBody = body,
            ClientIp = clientIp,
            Headers = headers,
            Method = method,
            Url = url
        };

    /// <inheritdoc />
    public void SetCorrelationId(string correlationId) => _result.CorrelationId = correlationId;

    /// <inheritdoc />
    public void SetStubExecutionResult(string stubId, bool passed, IEnumerable<ConditionCheckResultModel> conditions)
    {
        // Do not log the conditions with validation type "NotExecuted". They severely pollute the logging and API.
        conditions = conditions.Where(m => m.ConditionValidation != ConditionValidationType.NotExecuted);
        _result.StubExecutionResults.Add(new StubExecutionResultModel
        {
            Passed = passed,
            StubId = stubId,
            Conditions = conditions
        });
    }

    /// <inheritdoc />
    public void SetExecutingStubId(string stubId) => _result.ExecutingStubId = stubId;

    /// <inheritdoc />
    public void SetResponseWriterResult(StubResponseWriterResultModel result) =>
        _result.StubResponseWriterResults.Add(result);
}
