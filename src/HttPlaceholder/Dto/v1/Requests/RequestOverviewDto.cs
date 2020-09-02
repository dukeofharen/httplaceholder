using System;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using NJsonSchema.Infrastructure;

namespace HttPlaceholder.Dto.v1.Requests
{
    /// <summary>
    /// A model for storing the base properties of a request.
    /// </summary>
    public class RequestOverviewDto : IHaveCustomMapping //IMapFrom<RequestOverviewModel>
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

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

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<RequestOverviewModel, RequestOverviewDto>();
            configuration.CreateMap<RequestResultModel, RequestOverviewDto>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.RequestParameters.Method))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.RequestParameters.Url));
        }
    }
}
