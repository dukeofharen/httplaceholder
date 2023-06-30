using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.StubBuilders;
using HttPlaceholder.Client.Verification.Dto;
using HttPlaceholder.Client.Verification.Exceptions;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.Implementations;

/// <summary>
///     Describes a class that is used to communicate with HttPlaceholder.
/// </summary>
/// <example>
///     When you've initialized the client, you can call the HttPlaceholder API endpoints. Here is an example for how you
///     add a simple stub.
///     <code>
/// ...
/// var createdStub = await client.CreateStubAsync(new StubDto
/// {
/// Id = "test-stub-123",
/// Conditions = new StubConditionsDto
/// {
/// Method = "GET",
/// Url = new StubUrlConditionsDto
/// {
/// Path = "/test-path"
/// }
/// },
/// Response = new StubResponseDto
/// {
/// StatusCode = 200,
/// Json = @"{""key1"":""val1"", ""key2"":""val2""}"
/// }
/// });
/// ...
/// </code>
///     This method will create the stub and will also return the created stub. Because this way of adding stubs can get
///     very verbose very quick, another way of adding stubs with the client has been added: the StubBuilder. This is a
///     fluent builder which can also be used to create new stubs. Here is the same example, but now with using the
///     StubBuilder:
///     <code>
/// ...
/// var createdStub = await client.CreateStubAsync(StubBuilder.Begin()
/// .WithId("test-stub-123")
/// .WithConditions(StubConditionBuilder.Begin()
/// .WithHttpMethod(HttpMethod.Get)
/// .WithPath("/test-path"))
/// .WithResponse(StubResponseBuilder.Begin()
/// .WithHttpStatusCode(HttpStatusCode.Ok)
/// .WithJsonBody(new {key1 = "val1", key2 = "val2"})));
/// ...
/// </code>
///     This method is a bit shorter and is more readable.
/// </example>
public class HttPlaceholderClient : IHttPlaceholderClient
{
    private const string JsonContentType = "application/json";
    private const string TextContentType = "text/plain";
    private const string FromIdentifierHeaderKey = "x-from-identifier";
    private const string ItemsPerPageHeaderKey = "x-items-per-page";

    /// <summary>
    ///     Creates a <see cref="HttPlaceholderClient" /> instance.
    /// </summary>
    /// <param name="httpClient">A <see cref="HttpClient" /> instance.</param>
    public HttPlaceholderClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    internal HttpClient HttpClient { get; }

    /// <inheritdoc />
    public async Task<MetadataDto> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/metadata", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<MetadataDto>(content);
    }

    /// <inheritdoc />
    public async Task<bool> CheckFeatureAsync(FeatureFlagType featureFlag,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await HttpClient.GetAsync($"/ph-api/metadata/features/{featureFlag.ToString().ToLower()}",
                cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        var result = JsonConvert.DeserializeObject<FeatureResultDto>(content);
        return result is {Enabled: true};
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultDto>>
        GetAllRequestsAsync(CancellationToken cancellationToken = default) =>
        await GetAllRequestsAsync(null, 0, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync(
        string fromIdentifier,
        int numberOfRequestsPerPage,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/ph-api/requests");
        if (!string.IsNullOrWhiteSpace(fromIdentifier))
        {
            request.Headers.Add(FromIdentifierHeaderKey, fromIdentifier);
        }

        if (numberOfRequestsPerPage > 0)
        {
            request.Headers.Add(ItemsPerPageHeaderKey, numberOfRequestsPerPage.ToString());
        }

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<RequestResultDto>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync(
        CancellationToken cancellationToken = default) =>
        await GetRequestOverviewAsync(null, 0, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync(string fromIdentifier,
        int numberOfRequestsPerPage,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/ph-api/requests/overview");
        if (!string.IsNullOrWhiteSpace(fromIdentifier))
        {
            request.Headers.Add(FromIdentifierHeaderKey, fromIdentifier);
        }

        if (numberOfRequestsPerPage > 0)
        {
            request.Headers.Add(ItemsPerPageHeaderKey, numberOfRequestsPerPage.ToString());
        }

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<RequestOverviewDto>>(content);
    }

    /// <inheritdoc />
    public async Task<RequestResultDto> GetRequestAsync(string correlationId,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/requests/{correlationId}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<RequestResultDto>(content);
    }

    /// <inheritdoc />
    public async Task<ResponseDto> GetResponseAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/requests/{correlationId}/response", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<ResponseDto>(content);
    }

    /// <inheritdoc />
    public async Task DeleteAllRequestsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync("/ph-api/requests", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task DeleteRequestAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        using var response =
            await HttpClient.DeleteAsync($"/ph-api/requests/{correlationId}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task<FullStubDto> CreateStubForRequestAsync(string correlationId,
        CreateStubForRequestInputDto input = null, CancellationToken cancellationToken = default)
    {
        var body = input == null ? "{}" : JsonConvert.SerializeObject(input);
        using var response =
            await HttpClient.PostAsync($"/ph-api/requests/{correlationId}/stubs",
                new StringContent(body, Encoding.UTF8, JsonContentType), cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<FullStubDto>(content);
    }

    /// <inheritdoc />
    public async Task<FullStubDto> CreateStubAsync(StubDto stub, CancellationToken cancellationToken = default)
    {
        using var response =
            await HttpClient.PostAsync("/ph-api/stubs",
                new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType),
                cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<FullStubDto>(content);
    }

    /// <inheritdoc />
    public Task<FullStubDto> CreateStubAsync(StubBuilder stubBuilder, CancellationToken cancellationToken = default) =>
        CreateStubAsync(stubBuilder.Build(), cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubDto> stubs,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await HttpClient.PostAsync("/ph-api/stubs/multiple",
                new StringContent(JsonConvert.SerializeObject(stubs), Encoding.UTF8, JsonContentType),
                cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubBuilder> stubs,
        CancellationToken cancellationToken = default) =>
        CreateStubsAsync(stubs.Select(s => s.Build()), cancellationToken);

    /// <inheritdoc />
    public Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubDto[] stubs) =>
        CreateStubsAsync(stubs.AsEnumerable());

    /// <inheritdoc />
    public Task<IEnumerable<FullStubDto>>
        CreateStubsAsync(CancellationToken cancellationToken, params StubDto[] stubs) =>
        CreateStubsAsync(stubs.AsEnumerable(), cancellationToken);

    /// <inheritdoc />
    public Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubBuilder[] stubs) =>
        CreateStubsAsync(stubs.AsEnumerable());

    /// <inheritdoc />
    public Task<IEnumerable<FullStubDto>> CreateStubsAsync(CancellationToken cancellationToken,
        params StubBuilder[] stubs) => CreateStubsAsync(stubs.AsEnumerable(), cancellationToken);

    /// <inheritdoc />
    public async Task UpdateStubAsync(StubDto stub, string stubId, CancellationToken cancellationToken = default)
    {
        using var response =
            await HttpClient.PutAsync($"/ph-api/stubs/{stubId}",
                new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType),
                cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public Task UpdateStubAsync(StubBuilder stubBuilder, string stubId,
        CancellationToken cancellationToken = default) =>
        UpdateStubAsync(stubBuilder.Build(), stubId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> GetAllStubsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/stubs", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubOverviewDto>> GetStubOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/stubs/overview", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubOverviewDto>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultDto>> GetRequestsByStubIdAsync(string stubId,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/stubs/{stubId}/requests", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<RequestResultDto>>(content);
    }

    /// <inheritdoc />
    public async Task<FullStubDto> GetStubAsync(string stubId, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/stubs/{stubId}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<FullStubDto>(content);
    }

    /// <inheritdoc />
    public async Task DeleteStubAsync(string stubId, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync($"/ph-api/stubs/{stubId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllStubsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync("/ph-api/stubs", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetTenantNamesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/tenants", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<string>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> GetStubsByTenantAsync(string tenant,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/tenants/{tenant}/stubs", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public async Task DeleteAllStubsByTenantAsync(string tenant, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync($"/ph-api/tenants/{tenant}/stubs", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubDto> stubs,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PutAsync($"/ph-api/tenants/{tenant}/stubs",
            new StringContent(JsonConvert.SerializeObject(stubs), Encoding.UTF8, JsonContentType), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubBuilder> stubBuilders,
        CancellationToken cancellationToken = default) =>
        UpdateAllStubsByTenantAsync(tenant, stubBuilders.Select(b => b.Build()), cancellationToken);

    /// <inheritdoc />
    public async Task<UserDto> GetUserAsync(string username, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/users/{username}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<UserDto>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScenarioStateDto>> GetAllScenarioStatesAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/scenarios", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<ScenarioStateDto>>(content);
    }

    /// <inheritdoc />
    public async Task<ScenarioStateDto> GetScenarioStateAsync(string scenario,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync($"/ph-api/scenarios/{scenario}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<ScenarioStateDto>(content);
    }

    /// <inheritdoc />
    public async Task SetScenarioAsync(string scenario, ScenarioStateInputDto input,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PutAsync($"/ph-api/scenarios/{scenario}",
            new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, JsonContentType), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task DeleteScenarioAsync(string scenario, CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync($"/ph-api/scenarios/{scenario}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAllScenariosAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.DeleteAsync("/ph-api/scenarios", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> CreateCurlStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PostAsync(
            PrependImportQueryString("/ph-api/import/curl", model),
            new StringContent(model.Input,
                Encoding.UTF8,
                TextContentType), cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> CreateHarStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PostAsync(
            PrependImportQueryString("/ph-api/import/har", model),
            new StringContent(model.Input,
                Encoding.UTF8,
                TextContentType), cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubDto>> CreateOpenApiStubsAsync(ImportStubsModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PostAsync(
            PrependImportQueryString("/ph-api/import/openapi", model),
            new StringContent(model.Input,
                Encoding.UTF8,
                TextContentType), cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
    }

    /// <inheritdoc />
    public async Task<JobExecutionResultDto> ExecuteScheduledJobAsync(string jobName,
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.PostAsync(
            $"/ph-api/scheduledJob/{jobName}", new StringContent(string.Empty), cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<JobExecutionResultDto>(content);
    }

    /// <inheritdoc />
    public async Task<string[]> GetScheduledJobNamesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/scheduledJob", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<string[]>(content);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConfigurationDto>> GetConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await HttpClient.GetAsync("/ph-api/configuration", cancellationToken);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }

        return JsonConvert.DeserializeObject<IEnumerable<ConfigurationDto>>(content);
    }

    /// <inheritdoc />
    public async Task UpdateConfigurationValueAsync(UpdateConfigurationValueInputDto input,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"),
            "/ph-api/configuration")
        {
            Content = new StringContent(JsonConvert.SerializeObject(input),
                Encoding.UTF8,
                JsonContentType)
        };
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttPlaceholderClientException(response.StatusCode, content);
        }
    }

    /// <inheritdoc />
    public Task<VerificationResultModel> VerifyStubCalledAsync(string stubId,
        CancellationToken cancellationToken = default) =>
        VerifyStubCalledAsyncInternal(stubId, null, null, cancellationToken);

    /// <inheritdoc />
    public Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, TimesModel times,
        CancellationToken cancellationToken = default) =>
        VerifyStubCalledAsyncInternal(stubId, times, null, cancellationToken);

    /// <inheritdoc />
    public Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, TimesModel times,
        DateTime minimumRequestTime, CancellationToken cancellationToken = default) =>
        VerifyStubCalledAsyncInternal(stubId, times, minimumRequestTime, cancellationToken);

    /// <inheritdoc />
    public Task<VerificationResultModel> VerifyStubCalledAsync(string stubId, DateTime minimumRequestTime,
        CancellationToken cancellationToken = default) =>
        VerifyStubCalledAsyncInternal(stubId, null, minimumRequestTime, cancellationToken);

    internal static string PrependImportQueryString(string url, ImportStubsModel model)
    {
        var builder = new StringBuilder(url);
        builder.Append($"?doNotCreateStub={model.DoNotCreateStub}");
        if (!string.IsNullOrWhiteSpace(model.Tenant))
        {
            builder.Append($"&tenant={model.Tenant}");
        }

        if (!string.IsNullOrWhiteSpace(model.StubIdPrefix))
        {
            builder.Append($"&stubIdPrefix={model.StubIdPrefix}");
        }

        return builder.ToString();
    }

    internal async Task<VerificationResultModel> VerifyStubCalledAsyncInternal(
        string stubId,
        TimesModel times,
        DateTime? minimumRequestTime,
        CancellationToken cancellationToken = default)
    {
        var validationMessages = new List<string>();
        var requests = (await GetRequestsByStubIdAsync(stubId, cancellationToken)).ToArray();
        if (minimumRequestTime.HasValue)
        {
            requests = requests.Where(r => r.RequestBeginTime > minimumRequestTime).ToArray();
        }

        times ??= TimesModel.AtLeastOnce();
        if (times.ExactHits.HasValue && requests.Length != times.ExactHits)
        {
            validationMessages.Add($"Counted '{requests.Length}' requests, but expected '{times.ExactHits}'.");
        }
        else
        {
            if (times.MinHits.HasValue && requests.Length < times.MinHits)
            {
                validationMessages.Add($"Counted '{requests.Length}', but expected at least '{times.MinHits}'.");
            }

            if (times.MaxHits.HasValue && requests.Length > times.MaxHits)
            {
                validationMessages.Add($"Counted '{requests.Length}', but expected at most '{times.MaxHits}'.");
            }
        }

        var passed = !validationMessages.Any();
        var validationMessage = passed
            ? string.Empty
            : $"Validation failed. {string.Join(" ", validationMessages)}";
        var result = new VerificationResultModel {Message = validationMessage, Passed = passed, Requests = requests};
        if (!passed)
        {
            throw new StubVerificationFailedException(validationMessage, result);
        }

        return result;
    }
}
