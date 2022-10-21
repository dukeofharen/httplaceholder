using System;
using HttPlaceholder.Client.Verification.Dto;

namespace HttPlaceholder.Client.Verification.Exceptions;

/// <summary>
///     An exception that is thrown when stub request verification failed.
/// </summary>
public class StubVerificationFailedException : Exception
{
    /// <summary>
    ///     Constructs a <see cref="StubVerificationFailedException" /> instance.
    /// </summary>
    public StubVerificationFailedException(string message, VerificationResultModel resultModel) : base(message)
    {
        VerificationResultModel = resultModel;
    }

    /// <summary>
    ///     Gets or sets the verification result.
    /// </summary>
    public VerificationResultModel VerificationResultModel { get; }
}
