using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.TestConsoleApp
{
    [SuppressMessage("ReSharper", "All")]
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                services.AddHttPlaceholderClient(settings =>
                {
                    settings.BaseUrl = "http://localhost:5000";
                    settings.Username = "duco";
                    settings.Password = "pass";
                });
                var provider = services.BuildServiceProvider();
                var factory = provider.GetRequiredService<IHttPlaceholderClientFactory>();

                //var client = factory.MetadataClient;
                //var result = await client.GetAsync();

                var client = factory.RequestClient;
                var result = await client.GetAllAsync();
                var result2 = await client.GetByStubIdAsync("fallback");

                //var client = new StubClient(httpClient)
                //{
                //    BaseUrl = baseUrl
                //};
                //var result = await client.GetAllAsync();

                //var client = new TenantClient(httpClient)
                //{
                //    BaseUrl = baseUrl
                //};
                //var result = await client.GetAllAsync("01-get");
            }
            catch (Exception) { }
        }
    }
}
