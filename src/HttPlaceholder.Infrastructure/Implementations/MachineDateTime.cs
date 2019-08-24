using System;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations
{
    internal class MachineDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
