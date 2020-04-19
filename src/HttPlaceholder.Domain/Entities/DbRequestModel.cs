using System;
// ReSharper disable UnusedMember.Global

namespace HttPlaceholder.Domain.Entities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbRequestModel
    {
        public long Id { get; set; }

        public string CorrelationId { get; set; }

        public string ExecutingStubId { get; set; }

        public DateTime RequestBeginTime { get; set; }

        public DateTime RequestEndTime { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Json { get; set; }
    }
}
