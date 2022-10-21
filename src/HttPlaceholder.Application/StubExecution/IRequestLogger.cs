using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to log an HTTP request.
/// </summary>
public interface IRequestLogger
{
    /// <summary>
    ///     Sets the request correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID.</param>
    void SetCorrelationId(string correlationId);

    /// <summary>
    ///     Logs the request parameters.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="url">The request URL.</param>
    /// <param name="body">The request body.</param>
    /// <param name="clientIp">The IP address of the calling client.</param>
    /// <param name="headers">The request headers.</param>
    void LogRequestParameters(string method, string url, string body, string clientIp,
        IDictionary<string, string> headers);

    /// <summary>
    ///     Returns the built-up <see cref="RequestResultModel" />.
    /// </summary>
    /// <returns>The built-up <see cref="RequestResultModel" />.</returns>
    RequestResultModel GetResult();

    /// <summary>
    ///     Sets the stub execution result.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="passed">Whether any stub could be matched to the request.</param>
    /// <param name="conditions">A report of all the executed condition checkers.</param>
    void SetStubExecutionResult(string stubId, bool passed, IEnumerable<ConditionCheckResultModel> conditions);

    /// <summary>
    ///     Sets the stub response writer result.
    /// </summary>
    /// <param name="result">A report of the executed response writer.</param>
    void SetResponseWriterResult(StubResponseWriterResultModel result);

    /// <summary>
    ///     Sets the stub ID of the stub that was matched for this request.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    void SetExecutingStubId(string stubId);
}
