﻿using System;
using System.Collections.Generic;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing a request.
    /// </summary>
    public class RequestResultModel
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the request parameters.
        /// </summary>
        public RequestParametersModel RequestParameters { get; set; }

        /// <summary>
        /// Gets or sets the stub execution results.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public IList<StubExecutionResultModel> StubExecutionResults { get; set; } = new List<StubExecutionResultModel>();

        /// <summary>
        /// Gets or sets the stub response writer results.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public IList<StubResponseWriterResultModel> StubResponseWriterResults { get; set; } = new List<StubResponseWriterResultModel>();

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
