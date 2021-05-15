using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Dto.Stubs;

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

        /// <summary>
        /// Retrieves all requests made to HttPlaceholder as overview.
        /// </summary>
        /// <returns>The request overview.</returns>
        Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync();

        /// <summary>
        /// Retrieves a request by correlation ID.
        /// </summary>
        /// <param name="correlationId">The request correlation ID.</param>
        /// <returns>The request.</returns>
        Task<RequestResultDto> GetRequestAsync(string correlationId);

        /// <summary>
        /// Deletes all requests.
        /// </summary>
        Task DeleteAllRequestsAsync();

        /// <summary>
        /// Create a stub based on a specific request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request to create a stub for.</param>
        /// <returns>The created stub.</returns>
        Task<FullStubDto> CreateStubForRequestAsync(string correlationId);

        /// <summary>
        /// Create a new stub.
        /// </summary>
        /// <param name="stub">The stub to add.</param>
        /// <returns>The created stub.</returns>
        Task<FullStubDto> CreateStubAsync(StubDto stub);

        /// <summary>
        /// Update an existing stub.
        /// </summary>
        /// <param name="stub">The new stub contents.</param>
        /// <param name="stubId">The ID of the stub to update.</param>
        Task UpdateStubAsync(StubDto stub, string stubId);

        /// <summary>
        /// Get all stubs.
        /// </summary>
        /// <returns>A list of all stubs.</returns>
        Task<IEnumerable<FullStubDto>> GetAllStubsAsync();

        /// <summary>
        /// Get all stubs as overview.
        /// </summary>
        /// <returns>All stubs as overview.</returns>
        Task<IEnumerable<FullStubOverviewDto>> GetStubOverviewAsync();

        /// <summary>
        /// Get all requests based on stub ID.
        /// </summary>
        /// <param name="stubId">The stub ID.</param>
        /// <returns>The requests.</returns>
        Task<IEnumerable<RequestResultDto>> GetRequestsByStubIdAsync(string stubId);

        /// <summary>
        /// Get a specific stub
        /// </summary>
        /// <param name="stubId">The stub ID.</param>
        /// <returns>The stub.</returns>
        Task<FullStubDto> GetStubAsync(string stubId);

        /// <summary>
        /// Delete a specific stub.
        /// </summary>
        /// <param name="stubId">The stub ID.</param>
        Task DeleteStubAsync(string stubId);

        /// <summary>
        /// Delete all stubs.
        /// </summary>
        Task DeleteAllStubAsync();

        /// <summary>
        /// Get all tenant names.
        /// </summary>
        /// <returns>A list of tenant names.</returns>
        Task<IEnumerable<string>> GetTenantNamesAsync();
    }
}
