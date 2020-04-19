using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace HttPlaceholder.Dto.Requests
{
    /// <summary>
    /// A model for storing the request data for a request.
    /// </summary>
    public class RequestParametersDto : IMapFrom<RequestParametersModel>, IMapTo<RequestParametersModel>
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        public string ClientIp { get; set; }
    }
}
