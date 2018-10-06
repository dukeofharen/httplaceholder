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
using System.Collections.Generic;
using Newtonsoft.Json;

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
         IDictionary<string, string> argsDictionary;
         string configPath = Path.Join(AssemblyHelper.GetExecutingAssemblyRootPath(), "config.json");
         if (args.Length == 0 && File.Exists(configPath))
         {
            // If a config file is found, try to load and parse it instead of the arguments.
            Console.WriteLine($"Config file found at '{configPath}', so trying to parse that configuration.");
            string config = File.ReadAllText(configPath);
            argsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
         }
         else
         {
            Console.WriteLine("Trying to parse arguments from command line.");
            argsDictionary = args.Parse();
         }

         ConfigurationService.StaticSetConfiguration(argsDictionary);

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
