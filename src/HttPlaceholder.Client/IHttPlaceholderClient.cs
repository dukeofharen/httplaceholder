using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// Describes a class that is used to communicate with HttPlaceholder.
    /// </summary>
    public interface IHttPlaceholderClient
    {
        /// <summary>
        /// Retrieves the HttPlaceholder metadata.
        /// </summary>
        /// <returns>The metadata.</returns>
        Task<MetadataDto> GetMetadataAsync();

        /// <summary>
        /// Retrieves all requests made to HttPlaceholder.
        /// </summary>
        /// <returns>The requests.</returns>
        Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync();
    }
}
