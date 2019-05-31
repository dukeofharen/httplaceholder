using System;

namespace HttPlaceholder.Domain.Entities
{
    public class DbRequestModel
    {
        public long Id { get; set; }

        public string CorrelationId { get; set; }

        public string ExecutingStubId { get; set; }

        public DateTime RequestBeginTime { get; set; }

        public DateTime RequestEndTime { get; set; }

        public string Json { get; set; }
    }
}
