using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker to validate the HTTP method.
/// </summary>
public class MethodConditionChecker(IHttpContextService httpContextService) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var condition = stub.Conditions?.Method;
        if (condition == null)
        {
            return Task.FromResult(result);
        }

        var method = httpContextService.Method;
        if ((condition is string methodCondition &&
             string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase)) || (condition is not string &&
                ConversionUtilities
                    .ConvertEnumerable<string>(condition)
                    .Any(mc => string.Equals(mc, method, StringComparison.OrdinalIgnoreCase))))
        {
            // The path matches the provided condition. Add the stub ID to the resulting list.
            result.ConditionValidation = ConditionValidationType.Valid;
        }
        else
        {
            result.Log = $"Condition '{condition}' did not pass for request.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
