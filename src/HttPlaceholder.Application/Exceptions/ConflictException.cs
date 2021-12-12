using System;
using System.Runtime.Serialization;

namespace HttPlaceholder.Application.Exceptions;

[Serializable]
public class ConflictException : Exception
{
    public ConflictException(string message) : base($"Conflict detected: {message}")
    {
    }

    protected ConflictException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
        base(serializationInfo, streamingContext)
    {
    }
}