using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies the incoming request body.
/// </summary>
public class BodyConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub)
    {
        var bodyConditions = stub.Conditions?.Body?.ToArray();
        return bodyConditions is { Length: > 0 };
    }

    /// <inheritdoc />
    protected override async Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var bodyConditions = stub.Conditions.Body.ToArray();
        var body = await httpContextService.GetBodyAsync(cancellationToken);

        var validBodyConditions = 0;
        var log = string.Empty;
        foreach (var condition in bodyConditions)
        {
            if (!stringChecker.CheckString(body, condition, out var outputForLogging))
            {
                // If the check failed, it means the body condition is incorrect and the condition should fail.
                log = $"Body condition '{outputForLogging}' failed.";
                break;
            }

            validBodyConditions++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the body condition is passed and the stub ID is passed to the result.
        return validBodyConditions == bodyConditions.Length
            ? await ValidAsync(log)
            : await InvalidAsync(log);
    }

    /// <inheritdoc />
    public override int Priority => 8;
}
