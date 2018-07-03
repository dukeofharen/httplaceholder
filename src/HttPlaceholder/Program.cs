using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using HttPlaceholder.Models;
using HttPlaceholder.Services.Implementations;
using HttPlaceholder.Utilities;

namespace HttPlaceholder
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
         ConfigurationService.SetConfiguration(argsDictionary);

         int port = argsDictionary.GetValue(Constants.ConfigKeys.PortKey, 5000);
         string defaultPfxPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "key.pfx");
         string pfxPath = argsDictionary.GetValue(Constants.ConfigKeys.PfxPathKey, defaultPfxPath);
         string pfxPassword = argsDictionary.GetValue(Constants.ConfigKeys.PfxPasswordKey, "1234");
         int httpsPort = argsDictionary.GetValue(Constants.ConfigKeys.HttpsPortKey, 5050);
         bool useHttps = argsDictionary.GetValue(Constants.ConfigKeys.UseHttpsKey, false);
         return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
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
