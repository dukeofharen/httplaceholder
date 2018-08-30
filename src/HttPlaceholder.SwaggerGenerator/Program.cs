using HttPlaceholder.Services.Implementations;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HttPlaceholder.SwaggerGenerator
{
   class Program
   {
      static async Task Main(string[] args)
      {
         // Set dummy configuration.
         ConfigurationService.SetConfiguration(new Dictionary<string, string>());

         // This program hosts HttPlaceholder in memory, retrieves the contents of the swagger.json file and saves it.
         var startup = new Startup();
         var testServer = new TestServer(
           new WebHostBuilder()
            .ConfigureServices(services => Startup.ConfigureServicesStatic(services))
            .Configure(appBuilder => Startup.ConfigureStatic(appBuilder, null, false, false)));
         var client = testServer.CreateClient();

         // Retrieve the Swagger URL.
         using (var response = await client.GetAsync("swagger/v1/swagger.json"))
         {
            if (!response.IsSuccessStatusCode)
            {
               throw new Exception($"The call to the swagger.json URL failed with an HTTP '{response.StatusCode}'.");
            }

            string content = await response.Content.ReadAsStringAsync();
            string pathToSave = Path.Join(AssemblyHelper.GetExecutingAssemblyRootPath(), "swagger.json");
            File.WriteAllText(pathToSave, content);
         }
      }
   }
}
