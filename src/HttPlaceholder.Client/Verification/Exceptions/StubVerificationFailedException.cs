using System;

namespace HttPlaceholder.Client.Verification.Exceptions;

/// <summary>
/// An exception that is thrown when stub request verification failed.
/// </summary>
public class StubVerificationFailedException : Exception
{
    /// <summary>
    /// Constructs a <see cref="StubVerificationFailedException"/> instance.
    /// </summary>
    public StubVerificationFailedException(string message) : base(message)
    {
    }
}
