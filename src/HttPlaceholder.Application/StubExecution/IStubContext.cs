using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to communicate with the backing stub sources.
/// </summary>
public interface IStubContext
{
    /// <summary>
    /// Returns a list of <see cref="FullStubModel"/>.
    /// </summary>
    /// <returns>A list of <see cref="FullStubModel"/>.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsAsync();

    /// <summary>
    /// Returns a list of <see cref="FullStubModel"/> from read-only sources.
    /// </summary>
    /// <returns>A list of <see cref="FullStubModel"/> from read-only sources.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsFromReadOnlySourcesAsync();

    /// <summary>
    /// Returns a list of <see cref="FullStubModel"/> by tenant.
    /// </summary>
    /// <param name="tenant">The tenant name.</param>
    /// <returns>A list of <see cref="FullStubModel"/> by tenant.</returns>
    Task<IEnumerable<FullStubModel>> GetStubsAsync(string tenant);

    /// <summary>
    /// Returns a list of <see cref="FullStubOverviewModel"/>.
    /// </summary>
    /// <returns>A list of <see cref="FullStubOverviewModel"/>.</returns>
    Task<IEnumerable<FullStubOverviewModel>> GetStubsOverviewAsync();

    /// <summary>
    /// Adds a stub.
    /// </summary>
    /// <param name="stub">The <see cref="StubModel"/> to add.</param>
    /// <returns>The added <see cref="FullStubModel"/>.</returns>
    Task<FullStubModel> AddStubAsync(StubModel stub);

    /// <summary>
    /// Deletes a stub by stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>True if the stub was deleted, false otherwise.</returns>
    Task<bool> DeleteStubAsync(string stubId);

    /// <summary>
    /// Deletes all stubs by tenant.
    /// </summary>
    /// <param name="tenant">The tenant name.</param>
    Task DeleteAllStubsAsync(string tenant);

    /// <summary>
    /// Deletes all stubs.
    /// </summary>
    Task DeleteAllStubsAsync();

    /// <summary>
    /// Updates all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant name.</param>
    /// <param name="stubs">The stubs that should be updated.</param>
    Task UpdateAllStubs(string tenant, IEnumerable<StubModel> stubs);

    /// <summary>
    /// Retrieves a stub by stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>The <see cref="FullStubModel"/>.</returns>
    Task<FullStubModel> GetStubAsync(string stubId);

    /// <summary>
    /// Adds a <see cref="RequestResultModel"/>.
    /// </summary>
    /// <param name="requestResult">The request to add.</param>
    Task AddRequestResultAsync(RequestResultModel requestResult);

    /// <summary>
    /// Retrieves a list of <see cref="RequestResultModel"/>.
    /// </summary>
    /// <returns>A list of <see cref="RequestResultModel"/>.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

    /// <summary>
    /// Retrieves a list of <see cref="RequestOverviewModel"/>.
    /// </summary>
    /// <returns>A list of <see cref="RequestOverviewModel"/>.</returns>
    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync();

    /// <summary>
    /// Retrieves a list of <see cref="RequestResultModel"/> by specific stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>A list of <see cref="RequestResultModel"/>.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsByStubIdAsync(string stubId);

    /// <summary>
    /// Retrieves a specific <see cref="RequestResultModel"/> based on correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>A <see cref="RequestResultModel"/>.</returns>
    Task<RequestResultModel> GetRequestResultAsync(string correlationId);

    /// <summary>
    /// Deletes all requests.
    /// </summary>
    Task DeleteAllRequestResultsAsync();

    /// <summary>
    /// Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>True if the request was deleted, false otherwise.</returns>
    Task<bool> DeleteRequestAsync(string correlationId);

    /// <summary>
    /// Clean all old requests.
    /// </summary>
    Task CleanOldRequestResultsAsync();

    /// <summary>
    /// Retrieves a list with all tenant names.
    /// </summary>
    /// <returns>A list with all tenant names.</returns>
    Task<IEnumerable<string>> GetTenantNamesAsync();

    /// <summary>
    /// Saves a response.
    /// </summary>
    /// <param name="responseModel">The response to be saved.</param>
    Task SaveResponseAsync(ResponseModel responseModel);

    /// <summary>
    /// Prepares all stub sources.
    /// </summary>
    Task PrepareAsync();
}
