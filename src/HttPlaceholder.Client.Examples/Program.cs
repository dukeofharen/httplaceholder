using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Scenarios;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Client.Examples;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddHttPlaceholderClient(c =>
        {
            c.RootUrl = "http://localhost:5000";
            c.Username = "user";
            c.Password = "pass";
        });
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            var client = serviceProvider.GetRequiredService<IHttPlaceholderClient>();

            // Get the metadata.
            var metadata = await client.GetMetadataAsync();

            // Check if a feature is enabled.
            var authEnabled = await client.CheckFeatureAsync(FeatureFlagType.Authentication);

            // Get all requests.
            var allRequests = await client.GetAllRequestsAsync();

            // Get request overview.
            var requestOverview = await client.GetRequestOverviewAsync();

            // Get request.
            var request = await client.GetRequestAsync(requestOverview.First().CorrelationId);

            // Create stub for request.
            var createdStubFromRequest =
                await client.CreateStubForRequestAsync(requestOverview.First().CorrelationId);

            // Create stub.
            var createdStub = await client.CreateStubAsync(new StubDto
            {
                Id = "test-stub-123", Response = new StubResponseDto {Text = "Let's go yeah!"}
            });

            // Create stub using a builder.
            var createdStubFromBuilder = await client.CreateStubAsync(StubBuilder.Begin()
                .WithId("test-stub-124")
                .WithTenant("tenant1")
                .WithConditions(StubConditionBuilder.Begin()
                    .WithPath("/test-path")
                    .WithHttpMethod(HttpMethod.Get))
                .WithResponse(StubResponseBuilder.Begin()
                    .WithJsonBody(new {key1 = "val1", key2 = "val2"})));

            // Create multiple stubs.
            var createdStubs = await client.CreateStubsAsync(
                new StubDto {Id = "test-stub-1", Response = new StubResponseDto {Text = "Let's go yeah!"}},
                new StubDto {Id = "test-stub-2", Response = new StubResponseDto {Text = "Let's go yeah!"}});

            // Update stub.
            await client.UpdateStubAsync(
                new StubDto {Id = "test-stub-123", Response = new StubResponseDto {Text = "Let's go yeah!!"}},
                "test-stub-123");

            // Get all stubs.
            var allStubs = await client.GetAllStubsAsync();

            // Get stub overview.
            var stubOverview = await client.GetStubOverviewAsync();

            // Get requests by stub.
            var requestsForStub = await client.GetRequestsByStubIdAsync("test-stub-123");

            // Get specific stub.
            var stub = await client.GetStubAsync("test-stub-123");

            // Delete specific stub.
            await client.DeleteStubAsync("test-stub-123");


            // Get tenant names.
            var tenantNames = await client.GetTenantNamesAsync();

            // Create all stubs in tenant.
            await client.UpdateAllStubsByTenantAsync("test-tenant",
                new[]
                {
                    new StubDto {Id = "tenant-stub-1", Response = new StubResponseDto {Text = "response 1"}},
                    new StubDto {Id = "tenant-stub-2", Response = new StubResponseDto {Text = "response 2"}}
                });

            // Get stubs by tenant.
            var stubsByTenant = await client.GetStubsByTenantAsync("test-tenant");

            // Delete stubs by tenant.
            await client.DeleteAllStubsByTenantAsync("test-tenant");

            // Get user.
            var user = await client.GetUserAsync("user");

            // Delete all stubs.
            // await client.DeleteAllStubsAsync();

            // Delete all requests.
            // await client.DeleteAllRequestsAsync();

            // Delete request.
            await client.DeleteRequestAsync("6c449313-9871-4d1a-84ab-61f604667a06");

            // Set scenario.
            await client.SetScenarioAsync("scenario-1",
                new ScenarioStateInputDto {State = "new-state", HitCount = 1});

            // Get scenario.
            var scenarioState = await client.GetScenarioStateAsync("scenario-1");

            // Get all scenarios.
            var allScenarioStates = await client.GetAllScenarioStatesAsync();

            // Delete scenario state.
            await client.DeleteScenarioAsync("scenario-1");

            // Delete all scenario states.
            await client.DeleteAllScenariosAsync();

            // Create stubs based on cURL commands.
            const string commands = @"curl 'https://site.com/_nuxt/fonts/fa-solid-900.3eb06c7.woff2' \
  -H 'sec-ch-ua: "" Not A;Brand"";v=""99"", ""Chromium"";v=""96"", ""Google Chrome"";v=""96""' \
  -H 'Referer: ' \
  -H 'Origin: https://site.com' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'User-Agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36' \
  -H 'sec-ch-ua-platform: ""Linux""' \
  --compressed ;
curl 'https://site.com/_nuxt/css/4cda201.css' \
  -H 'authority: site.com' \
  -H 'sec-ch-ua: "" Not A;Brand"";v=""99"", ""Chromium"";v=""96"", ""Google Chrome"";v=""96""' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'user-agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36' \
  -H 'sec-ch-ua-platform: ""Linux""' \
  -H 'accept: text/css,*/*;q=0.1' \
  -H 'sec-fetch-site: same-origin' \
  -H 'sec-fetch-mode: no-cors' \
  -H 'sec-fetch-dest: style' \
  -H 'accept-language: en-US,en;q=0.9,nl;q=0.8' \
  -H 'cookie: Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=' \
  --compressed ;
curl 'https://site.com/_nuxt/1d6c3a9.js' \
  -H 'authority: site.com' \
  -H 'sec-ch-ua: "" Not A;Brand"";v=""99"", ""Chromium"";v=""96"", ""Google Chrome"";v=""96""' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'user-agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36' \
  -H 'sec-ch-ua-platform: ""Linux""' \
  -H 'accept: */*' \
  -H 'sec-fetch-site: same-origin' \
  -H 'sec-fetch-mode: no-cors' \
  -H 'sec-fetch-dest: script' \
  -H 'accept-language: en-US,en;q=0.9,nl;q=0.8' \
  -H 'cookie: Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=' \
  --compressed";
            var curlCommandResult = await client.CreateCurlStubsAsync(commands, false);

            // Create stubs based on HTTP archive (HAR).
            var har = await File.ReadAllTextAsync("har.json");
            var harResult = await client.CreateHarStubsAsync(har, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
