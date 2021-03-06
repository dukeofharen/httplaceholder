﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Dto.Users;
using HttPlaceholder.Client.StubBuilders;

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
        /// Check whether a specific feature is enabled or not.
        /// </summary>
        /// <param name="featureFlag">The feature flag to check.</param>
        /// <returns>True when the feature is enabled; false otherwise.</returns>
        Task<bool> CheckFeatureAsync(FeatureFlagType featureFlag);

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
        /// Deletes a specific request.
        /// </summary>
        /// <param name="correlationId">The request correlation ID.</param>
        Task DeleteRequestAsync(string correlationId);

        /// <summary>
        /// Create a stub based on a specific request.
        /// </summary>
        /// <param name="correlationId">The correlation ID of the request to create a stub for.</param>
        /// <param name="input">The input for specifying the options for the stub.</param>
        /// <returns>The created stub.</returns>
        Task<FullStubDto> CreateStubForRequestAsync(string correlationId, CreateStubForRequestInputDto input = null);

        /// <summary>
        /// Create a new stub.
        /// </summary>
        /// <param name="stub">The stub to add.</param>
        /// <returns>The created stub.</returns>
        Task<FullStubDto> CreateStubAsync(StubDto stub);

        /// <summary>
        /// Create a new stub.
        /// </summary>
        /// <param name="stubBuilder">The stub to add.</param>
        /// <returns>The created stub.</returns>
        Task<FullStubDto> CreateStubAsync(StubBuilder stubBuilder);

        /// <summary>
        /// Update an existing stub.
        /// </summary>
        /// <param name="stub">The new stub contents.</param>
        /// <param name="stubId">The ID of the stub to update.</param>
        Task UpdateStubAsync(StubDto stub, string stubId);

        /// <summary>
        /// Update an existing stub.
        /// </summary>
        /// <param name="stubBuilder">The new stub contents.</param>
        /// <param name="stubId">The ID of the stub to update.</param>
        Task UpdateStubAsync(StubBuilder stubBuilder, string stubId);

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
        Task DeleteAllStubsAsync();

        /// <summary>
        /// Get all tenant names.
        /// </summary>
        /// <returns>A list of tenant names.</returns>
        Task<IEnumerable<string>> GetTenantNamesAsync();

        /// <summary>
        /// Get all stubs belonging to a specific tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <returns>The stubs.</returns>
        Task<IEnumerable<FullStubDto>> GetStubsByTenantAsync(string tenant);

        /// <summary>
        /// Delete all stubs belonging to a specific tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        Task DeleteAllStubsByTenantAsync(string tenant);

        /// <summary>
        /// Update all stubs belonging to a specific tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="stubs">The stubs.</param>
        Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubDto> stubs);

        /// <summary>
        /// Update all stubs belonging to a specific tenant.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="stubBuilders">The stubs.</param>
        Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubBuilder> stubBuilders);

        /// <summary>
        /// Get the current user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user.</returns>
        Task<UserDto> GetUserAsync(string username);
    }
}
