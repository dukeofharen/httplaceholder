using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Middleware;
using Placeholder.Utilities;
using YamlDotNet.Serialization;

namespace Placeholder
{
   public class Startup
   {
      private readonly IConfiguration _configuration;

      public Startup(IConfiguration configuration)
      {
         _configuration = configuration;
      }

      public void ConfigureServices(IServiceCollection services)
      {
         string inputFileLocation = _configuration["inputFile"];
         if (string.IsNullOrEmpty(inputFileLocation))
         {
            Console.WriteLine(@"'inputFile' parameter not passed to tool. Start the application like this: placeholder --inputFile C:\tmp\stub.yml");
            Environment.Exit(-1);
         }
         else if (!File.Exists(inputFileLocation))
         {
            Console.WriteLine($"Input file '{inputFileLocation}' not found.");
            Environment.Exit(-1);
         }

         // Load and parse the input YAML file here.
         string inputFile = File.ReadAllText(inputFileLocation);
         var yaml = YamlHelper.Parse(inputFile);
         if (!(yaml is IList<object> stubList))
         {
            Console.WriteLine($"Input file '{inputFileLocation}' is not a valid Placeholder YAML file.");
         }

         services.AddMvc();
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         app.UseMiddleware<StubHandlingMiddleware>();
         app.UseMvc();
      }
   }
}
