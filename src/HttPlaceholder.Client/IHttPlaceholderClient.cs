using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// Describes a class that is used to make REST calls to the HttPlaceholder REST API.
    /// </summary>
    public interface IHttPlaceholderClient
    {
        /// <summary>
        /// Checks whether the given credentials belong to a valid user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="UserModel"/>.</returns>
        /// <remarks>
        /// If no authentication is configured on HttPlaceholder, this call will always return a successful result.
        /// </remarks>
        Task<UserModel> GetUserAsync(string username, string password);

        /// <summary>
        /// Retrieves the HttPlaceholder metadata.
        /// </summary>
        /// <returns>The <see cref="MetadataModel"/>.</returns>
        Task<MetadataModel> GetMetadataAsync();

        /// <summary>
        /// Retrieves all requests.
        /// </summary>
        /// <returns>A list of <see cref="RequestResultModel"/>.</returns>
        Task<IEnumerable<RequestResultModel>> GetAllRequestsAsync();

        /// <summary>
        /// Retrieves all request made for a specific stub.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        /// <returns>
        /// A list of <see cref="RequestResultModel" />.
        /// </returns>
        Task<IEnumerable<RequestResultModel>> GetAllRequestsByStubIdAsync(string stubId);

        /// <summary>
        /// Deletes all requests.
        /// </summary>
        Task DeleteAllRequestsAsync();

        /// <summary>
        /// Retrieves a stub by ID.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        /// <returns>The <see cref="FullStubModel"/>.</returns>
        Task<FullStubModel> GetStubAsync(string stubId);

        /// <summary>
        /// Retrieves all stubs.
        /// </summary>
        /// <returns>A list of <see cref="FullStubModel"/>.</returns>
        Task<IEnumerable<FullStubModel>> GetAllStubsAsync();

        /// <summary>
        /// Adds a stub.
        /// </summary>
        /// <param name="stub">The stub.</param>
        /// <remarks>
        /// If a stub with the same ID already exists, it will be overwritten.
        /// </remarks>
        Task AddStubAsync(StubModel stub);

        /// <summary>
        /// Deletes a stub.
        /// </summary>
        /// <param name="stubId">The stub identifier.</param>
        Task DeleteStubAsync(string stubId);

        /// <summary>
        /// Retrieve all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <returns>A list of <see cref="FullStubModel"/>.</returns>
        Task<IEnumerable<FullStubModel>> GetAllStubsInTenant(string tenant);

        /// <summary>
        /// Delete all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        Task DeleteAllStubsInTenant(string tenant);

        /// <summary>
        /// Update all stubs in a given tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="stubs">The stubs.</param>
        Task UpdateAllStubsInTenant(string tenant, IEnumerable<StubModel> stubs);
    }
}
