using System;

namespace HttPlaceholder.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base($"Conflict detected: {message}")
        {
        }
    }
}
