using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Stubs;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Client.Examples
{
    static class Program
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
