﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies the incoming request body.
/// </summary>
public class BodyConditionChecker : IConditionChecker, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    ///     Constructs a <see cref="BodyConditionChecker" /> instance.
    /// </summary>
    public BodyConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    {
        _httpContextService = httpContextService;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var bodyConditions = stub.Conditions?.Body?.ToArray();
        if (bodyConditions == null || bodyConditions?.Any() != true)
        {
            return result;
        }

        var body = await _httpContextService.GetBodyAsync(cancellationToken);

        var validBodyConditions = 0;
        foreach (var condition in bodyConditions)
        {
            if (!_stringChecker.CheckString(body, condition, out var outputForLogging))
            {
                // If the check failed, it means the body condition is incorrect and the condition should fail.
                result.Log = $"Body condition '{outputForLogging}' failed.";
                break;
            }

            validBodyConditions++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the body condition is passed and the stub ID is passed to the result.
        result.ConditionValidation = validBodyConditions == bodyConditions.Length
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;

        return result;
    }

    /// <inheritdoc />
    public int Priority => 8;
}
