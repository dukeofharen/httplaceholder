using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Configuration;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Import;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Dto.Scenarios;
using HttPlaceholder.Client.Dto.ScheduledJobs;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Dto.Users;
using HttPlaceholder.Client.StubBuilders;
using HttPlaceholder.Client.Verification.Dto;

namespace HttPlaceholder.Client;

/// <summary>
///     Describes a class that is used to communicate with HttPlaceholder.
/// </summary>
public interface IHttPlaceholderClient
{
    /// <summary>
    ///     Retrieves the HttPlaceholder metadata.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The metadata.</returns>
    Task<MetadataDto> GetMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Check whether a specific feature is enabled or not.
    /// </summary>
    /// <param name="featureFlag">The feature flag to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the feature is enabled; false otherwise.</returns>
    Task<bool> CheckFeatureAsync(FeatureFlagType featureFlag, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all requests made to HttPlaceholder.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requests.</returns>
    Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all requests made to HttPlaceholder as overview.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request overview.</returns>
    Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a request by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    Task<RequestResultDto> GetRequestAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a response by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    Task<ResponseDto> GetResponseAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteRequestAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a stub based on a specific request.
    /// </summary>
    /// <param name="correlationId">The correlation ID of the request to create a stub for.</param>
    /// <param name="input">The input for specifying the options for the stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stub.</returns>
    Task<FullStubDto> CreateStubForRequestAsync(string correlationId, CreateStubForRequestInputDto input = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new stub.
    /// </summary>
    /// <param name="stub">The stub to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stub.</returns>
    Task<FullStubDto> CreateStubAsync(StubDto stub, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new stub.
    /// </summary>
    /// <param name="stubBuilder">The stub to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stub.</returns>
    Task<FullStubDto> CreateStubAsync(StubBuilder stubBuilder, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubDto> stubs,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubBuilder> stubs,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubDto[] stubs);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(CancellationToken cancellationToken, params StubDto[] stubs);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubBuilder[] stubs);

    /// <summary>
    ///     Creates at least one new stub.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateStubsAsync(CancellationToken cancellationToken, params StubBuilder[] stubs);

    /// <summary>
    ///     Update an existing stub.
    /// </summary>
    /// <param name="stub">The new stub contents.</param>
    /// <param name="stubId">The ID of the stub to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateStubAsync(StubDto stub, string stubId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update an existing stub.
    /// </summary>
    /// <param name="stubBuilder">The new stub contents.</param>
    /// <param name="stubId">The ID of the stub to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateStubAsync(StubBuilder stubBuilder, string stubId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all stubs.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all stubs.</returns>
    Task<IEnumerable<FullStubDto>> GetAllStubsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all stubs as overview.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stubs as overview.</returns>
    Task<IEnumerable<FullStubOverviewDto>> GetStubOverviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all requests based on stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requests.</returns>
    Task<IEnumerable<RequestResultDto>> GetRequestsByStubIdAsync(string stubId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get a specific stub
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stub.</returns>
    Task<FullStubDto> GetStubAsync(string stubId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete a specific stub.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteStubAsync(string stubId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete all stubs.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllStubsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all tenant names.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of tenant names.</returns>
    Task<IEnumerable<string>> GetTenantNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all stubs belonging to a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stubs.</returns>
    Task<IEnumerable<FullStubDto>> GetStubsByTenantAsync(string tenant, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete all stubs belonging to a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllStubsByTenantAsync(string tenant, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update all stubs belonging to a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="stubs">The stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubDto> stubs,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update all stubs belonging to a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="stubBuilders">The stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubBuilder> stubBuilders,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get the current user by username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user.</returns>
    Task<UserDto> GetUserAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the states of all scenarios.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The scenario states.</returns>
    Task<IEnumerable<ScenarioStateDto>> GetAllScenarioStatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the state of a specific scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The scenario state.</returns>
    Task<ScenarioStateDto> GetScenarioStateAsync(string scenario, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sets the scenario state to a new value.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="input">The new state.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetScenarioAsync(string scenario, ScenarioStateInputDto input, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes / clears a scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteScenarioAsync(string scenario, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all scenarios.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllScenariosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates stubs based on cURL commands.
    /// </summary>
    /// <param name="model">The model that contains the parameters for importing the stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateCurlStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates stubs based on an HTTP archive (HAR)
    /// </summary>
    /// <param name="model">The model that contains the parameters for importing the stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateHarStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates stubs based on an OpenAPI definition (both JSON and YAML supported).
    /// </summary>
    /// <param name="model">The model that contains the parameters for importing the stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created stubs.</returns>
    Task<IEnumerable<FullStubDto>> CreateOpenApiStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a given scheduled job.
    /// </summary>
    /// <param name="jobName">The name of the job.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="JobExecutionResultDto" /> with the execution results.</returns>
    Task<JobExecutionResultDto> ExecuteScheduledJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of scheduled job names.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An array containing all active scheduled job names.</returns>
    Task<string[]> GetScheduledJobNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of configuration items.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>List of <see cref="ConfigurationDto" />.</returns>
    Task<IEnumerable<ConfigurationDto>> GetConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a given configuration value.
    /// </summary>
    /// <param name="input">The configuration input/</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateConfigurationValueAsync(UpdateConfigurationValueInputDto input,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Verifies that a stub with the specified stubId has been called.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="VerificationResultModel" />.</returns>
    Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Verifies that a stub with the specified stubId has been called.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="times">A model to verify the number of times a stub has been called.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="VerificationResultModel" />.</returns>
    Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, TimesModel times,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Verifies that a stub with the specified stubId has been called.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="times">A model to verify the number of times a stub has been called.</param>
    /// <param name="minimumRequestTime">The minimum date/time in UTC the request(s) should have been called.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="VerificationResultModel" />.</returns>
    Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, TimesModel times, DateTime minimumRequestTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Verifies that a stub with the specified stubId has been called.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="minimumRequestTime">The minimum date/time in UTC the request(s) should have been called.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="VerificationResultModel" />.</returns>
    Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, DateTime minimumRequestTime,
        CancellationToken cancellationToken = default);
}
