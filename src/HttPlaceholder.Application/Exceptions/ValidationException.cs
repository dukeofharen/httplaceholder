﻿using System;
using System.Collections.Generic;

namespace HttPlaceholder.Application.Exceptions;

/// <summary>
///     An exception that is thrown when a validation exception occurs.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    ///     Constructs a <see cref="ValidationException" /> instance.
    /// </summary>
    /// <param name="validationErrors">The list of validation errors.</param>
    public ValidationException(IEnumerable<string> validationErrors)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    ///     Constructs a <see cref="ValidationException" /> instance.
    /// </summary>
    /// <param name="error">The error.</param>
    public ValidationException(string error)
    {
        ValidationErrors = new[] { error };
    }

    /// <summary>
    ///     Gets the validation errors.
    /// </summary>
    public IEnumerable<string> ValidationErrors { get; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    public override string Message => string.Format(ApplicationResources.ValidationFailed,
        string.Join(Environment.NewLine, ValidationErrors));
}
