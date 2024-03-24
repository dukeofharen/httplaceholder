﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to validate a posted form.
/// </summary>
public class FormValueConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var formConditions = stub.Conditions?.Form?.ToArray() ?? [];
        if (formConditions.Length == 0)
        {
            return await InvalidAsync();
        }

        var form = await httpContextService.GetFormValuesAsync(cancellationToken);
        var validConditions = 0;
        foreach (var condition in formConditions)
        {
            // Do a present check, if needed.
            if (condition.Value is not string)
            {
                var checkingModel = ConversionUtilities.Convert<StubConditionStringCheckingModel>(condition.Value);
                if (checkingModel.Present != null)
                {
                    if ((checkingModel.Present.Value && form.Any(f => f.Item1.Equals(condition.Key))) ||
                        (!checkingModel.Present.Value && !form.Any(f => f.Item1.Equals(condition.Key))))
                    {
                        validConditions++;
                    }

                    continue;
                }
            }

            var (formKey, formValues) = form.FirstOrDefault(f => f.Item1 == condition.Key);
            if (formKey == null)
            {
                return await InvalidAsync($"No form value with key '{condition.Key}' found.");
            }

            validConditions += formValues
                .Count(value => stringChecker.CheckString(HttpUtility.UrlDecode(value), condition.Value, out _));
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the form condition is passed and the stub ID is passed to the result.
        if (validConditions == formConditions.Length)
        {
            return await ValidAsync();
        }

        return await InvalidAsync(
            $"Number of configured form conditions: '{formConditions.Length}'; number of passed form conditions: '{validConditions}'");
    }

    /// <inheritdoc />
    public int Priority => 8;
}
