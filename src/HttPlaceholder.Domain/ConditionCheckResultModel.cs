﻿using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing a condition check result.
/// </summary>
public class ConditionCheckResultModel
{
    /// <summary>
    ///     Gets or sets the name of the checker.
    /// </summary>
    public string CheckerName { get; set; }

    /// <summary>
    ///     Gets or sets the condition validation.
    /// </summary>
    public ConditionValidationType ConditionValidation { get; set; } = ConditionValidationType.NotExecuted;

    /// <summary>
    ///     Gets or sets the log.
    /// </summary>
    public string Log { get; set; }

    /// <summary>
    ///     Sets the checker name.
    /// </summary>
    /// <param name="name">The checker name.</param>
    /// <returns>The current model.</returns>
    public ConditionCheckResultModel SetCheckerName(string name)
    {
        CheckerName = name;
        return this;
    }

    /// <summary>
    ///     Creates a valid result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static ConditionCheckResultModel Valid(string log = null) => CreateModel(ConditionValidationType.Valid, log);

    /// <summary>
    ///     Creates an invalid result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static ConditionCheckResultModel Invalid(string log = null) =>
        CreateModel(ConditionValidationType.Invalid, log);

    /// <summary>
    ///     Creates a not result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static ConditionCheckResultModel NotExecuted(string log = null) =>
        CreateModel(ConditionValidationType.NotExecuted, log);

    /// <summary>
    ///     Creates a valid result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static Task<ConditionCheckResultModel> ValidAsync(string log = null) => Valid(log).AsTask();

    /// <summary>
    ///     Creates an invalid result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static Task<ConditionCheckResultModel> InvalidAsync(string log = null) =>
        Invalid(log).AsTask();

    /// <summary>
    ///     Creates a not result.
    /// </summary>
    /// <param name="log">Optional log message.</param>
    /// <returns>The result model.</returns>
    public static Task<ConditionCheckResultModel> NotExecutedAsync(string log = null) =>
        NotExecuted(log).AsTask();

    private static ConditionCheckResultModel CreateModel(ConditionValidationType type, string log = null)
    {
        var model = new ConditionCheckResultModel { ConditionValidation = type };
        if (!string.IsNullOrWhiteSpace(log))
        {
            model.Log = log;
        }

        return model;
    }
}
