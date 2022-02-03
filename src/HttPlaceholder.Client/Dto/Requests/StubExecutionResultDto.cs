using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Requests
{
    /// <summary>
    /// A model for storing all execution related data for a given stub.
    /// </summary>
    public class StubExecutionResultDto
    {
        /// <summary>
        /// Gets or sets the stub identifier.
        /// </summary>
        public string StubId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StubExecutionResultDto"/> is passed.
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        public IEnumerable<ConditionCheckResultDto> Conditions { get; set; }
    }
}