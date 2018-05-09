using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Placeholder.Utilities;

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

      public static IWebHost BuildWebHost(string[] args)
      {
         var argsDictionary = args.Parse();
         int port = argsDictionary.GetValue("port", 5000);
         return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) => config.AddCommandLine(args))
            .UseKestrel(options =>
            {
               options.AddServerHeader = false;
               options.Listen(IPAddress.Loopback, port);
            })
            .Build();
      }
   }
}
