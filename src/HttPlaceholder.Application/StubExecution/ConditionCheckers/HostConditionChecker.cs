﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to verify the hostname.
/// </summary>
public class HostConditionChecker : IConditionChecker, ISingletonService
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    ///     Constructs a <see cref="HostConditionChecker" /> instance.
    /// </summary>
    public HostConditionChecker(IClientDataResolver clientDataResolver, IStringChecker stringChecker)
    {
        _clientDataResolver = clientDataResolver;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var hostCondition = stub.Conditions?.Host;
        if (hostCondition == null)
        {
            return Task.FromResult(result);
        }

        var host = _clientDataResolver.GetHost();
        result.ConditionValidation = !_stringChecker.CheckString(host, hostCondition, out _)
            ? ConditionValidationType.Invalid
            : ConditionValidationType.Valid;

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
