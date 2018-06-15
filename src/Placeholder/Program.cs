using System;
using System.IO;
using System.Net;
using System.Reflection;
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
         string defaultPfxPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "key.pfx");
         string pfxPath = argsDictionary.GetValue("pfxPath", defaultPfxPath);
         string pfxPassword = argsDictionary.GetValue("pfxPassword", "1234");
         int httpsPort = argsDictionary.GetValue("httpsPort", 5050);
         bool useHttps = argsDictionary.GetValue("useHttps", false);
         return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) => config.AddCommandLine(args))
            .UseKestrel(options =>
            {
               options.AddServerHeader = false;
               options.Listen(IPAddress.Any, port);
               if (useHttps && !string.IsNullOrWhiteSpace(pfxPath) && !string.IsNullOrWhiteSpace(pfxPassword))
               {
                  options.Listen(IPAddress.Any, httpsPort, listenOptions => listenOptions.UseHttps(pfxPath, pfxPassword));
               }
            })
            .Build();
      }
   }
}
