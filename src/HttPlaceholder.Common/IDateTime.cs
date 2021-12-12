using System;

namespace HttPlaceholder.Common;

public interface IDateTime
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}