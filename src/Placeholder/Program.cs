using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Placeholder
{
   class Program
   {
      public static void Main(string[] args)
      {
         try
         {
            BuildWebHost(args).Run();
         }
         catch (Exception e)
         {
            Console.WriteLine($"Unexpected exception thrown: {e}");
            Environment.Exit(-1);
         }
      }

      public static IWebHost BuildWebHost(string[] args) =>
         WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) => config.AddCommandLine(args))
            .UseKestrel(options => options.AddServerHeader = false)
            .Build();
   }
}
