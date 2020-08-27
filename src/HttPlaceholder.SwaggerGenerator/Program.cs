using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.SwaggerGenerator
{
    internal static class Program
    {
        // ReSharper disable once UnusedParameter.Global
        public static async Task Main(string[] args)
        {
            var pathToSave = Path.Combine(AssemblyHelper.GetCallingAssemblyRootPath(), "swagger.json");
            if (args.Any())
            {
                var firstArg = args[0];
                if (File.Exists(firstArg))
                {
                    pathToSave = firstArg;
                }
                else if (Directory.Exists(firstArg))
                {
                    pathToSave = Path.Combine(firstArg, "swagger.json");
                }
            }

            // Mock settings.
            var config = new ConfigurationBuilder().Build();

            // This program hosts HttPlaceholder in memory, retrieves the contents of the swagger.json file and saves it.
            var testServer = new TestServer(
              new WebHostBuilder()
               .ConfigureServices(services => Startup.ConfigureServicesStatic(services, config))
               .Configure(appBuilder => Startup.ConfigureStatic(appBuilder, false, false)));
            var client = testServer.CreateClient();

            // Retrieve the Swagger URL.
            using var response = await client.GetAsync("swagger/v1/swagger.json");
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"The call to the swagger.json URL failed with an HTTP '{response.StatusCode}'.");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($@"Saving Swagger file to {pathToSave}");
            await File.WriteAllTextAsync(pathToSave, content);
        }
    }
}
