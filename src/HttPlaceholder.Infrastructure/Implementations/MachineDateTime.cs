using System;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <inheritdoc />
internal class MachineDateTime : IDateTime
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}
