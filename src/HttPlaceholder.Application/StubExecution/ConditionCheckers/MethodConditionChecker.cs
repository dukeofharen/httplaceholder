using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker to validate the HTTP method.
/// </summary>
public class MethodConditionChecker(IHttpContextService httpContextService) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var condition = stub.Conditions?.Method;
        if (condition == null)
        {
            return NotExecutedAsync();
        }

        var method = httpContextService.Method;
        if ((condition is string methodCondition &&
             string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase)) || (condition is not string &&
                ConversionUtilities
                    .ConvertEnumerable<string>(condition)
                    .Any(mc => string.Equals(mc, method, StringComparison.OrdinalIgnoreCase))))
        {
            // The path matches the provided condition. Add the stub ID to the resulting list.
            return ValidAsync();
        }

        return InvalidAsync($"Condition '{condition}' did not pass for request.");
    }

    /// <inheritdoc />
    public int Priority => 10;
}
