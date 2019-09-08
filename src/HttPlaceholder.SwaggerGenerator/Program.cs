using System;
using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.SwaggerGenerator
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            // Mock settings.
            var config = new ConfigurationBuilder().Build();

            // This program hosts HttPlaceholder in memory, retrieves the contents of the swagger.json file and saves it.
            var testServer = new TestServer(
              new WebHostBuilder()
               .ConfigureServices(services => Startup.ConfigureServicesStatic(services, config))
               .Configure(appBuilder => Startup.ConfigureStatic(appBuilder, false, false)));
            var client = testServer.CreateClient();

            // Retrieve the Swagger URL.
            using (var response = await client.GetAsync("swagger/v1/swagger.json"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"The call to the swagger.json URL failed with an HTTP '{response.StatusCode}'.");
                }

                string content = await response.Content.ReadAsStringAsync();
                string pathToSave = Path.Join(AssemblyHelper.GetCallingAssemblyRootPath(), "swagger.json");
                File.WriteAllText(pathToSave, content);
            }
        }
    }
}
