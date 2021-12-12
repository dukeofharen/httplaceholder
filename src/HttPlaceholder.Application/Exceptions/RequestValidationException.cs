using System;
using System.Runtime.Serialization;

namespace HttPlaceholder.Application.Exceptions;

[Serializable]
public class RequestValidationException : Exception
{
    public RequestValidationException(string message) : base(message)
    {
    }

    protected RequestValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
        base(serializationInfo, streamingContext)
    {
    }
}