using System;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class MachineDateTime : IDateTime, ISingletonService
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public long UtcNowUnix => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
