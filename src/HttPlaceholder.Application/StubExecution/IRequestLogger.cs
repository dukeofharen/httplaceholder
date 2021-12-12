using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

public interface IRequestLogger
{
    void SetCorrelationId(string correlationId);

    void LogRequestParameters(string method, string url, string body, string clientIp, IDictionary<string, string> headers);

    RequestResultModel GetResult();

    void SetStubExecutionResult(string stubId, bool passed, IEnumerable<ConditionCheckResultModel> conditions);

    void SetResponseWriterResult(StubResponseWriterResultModel result);

    void SetExecutingStubId(string stubId);
}