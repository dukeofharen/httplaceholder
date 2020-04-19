using System;
using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace HttPlaceholder.Dto.Requests
{
    /// <summary>
    /// A model for storing a request.
    /// </summary>
    public class RequestResultDto : IMapFrom<RequestResultModel>, IMapTo<RequestResultModel>
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the request parameters.
        /// </summary>
        public RequestParametersDto RequestParameters { get; set; }

        /// <summary>
        /// Gets or sets the stub execution results.
        /// </summary>
        public IList<StubExecutionResultDto> StubExecutionResults { get; set; } = new List<StubExecutionResultDto>();

        /// <summary>
        /// Gets or sets the stub response writer results.
        /// </summary>
        public IList<StubResponseWriterResultDto> StubResponseWriterResults { get; set; } = new List<StubResponseWriterResultDto>();

        /// <summary>
        /// Gets or sets the executing stub identifier.
        /// </summary>
        public string ExecutingStubId { get; set; }

        /// <summary>
        /// Gets or sets the tenant name of the stub.
        /// </summary>
        public string StubTenant { get; set; }

        /// <summary>
        /// Gets or sets the request begin time.
        /// </summary>
        public DateTime RequestBeginTime { get; set; }

        /// <summary>
        /// Gets or sets the request end time.
        /// </summary>
        public DateTime RequestEndTime { get; set; }
    }
}
