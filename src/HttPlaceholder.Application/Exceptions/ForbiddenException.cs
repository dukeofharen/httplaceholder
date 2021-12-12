using System;
using System.Runtime.Serialization;

namespace HttPlaceholder.Application.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    public ForbiddenException()
    {
    }

    protected ForbiddenException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
        base(serializationInfo, streamingContext)
    {
    }
}