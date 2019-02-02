using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Console;
using HttPlaceholder.Models;
using HttPlaceholder.Resources;
using HttPlaceholder.Services.Implementations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HttPlaceholder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"HttPlaceholder {AssemblyHelper.GetAssemblyVersion()} - (c) 2018 Ducode");

            var arg = args.FirstOrDefault();
            if (string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(ConsoleAppResources.ManPage);
                Environment.Exit(0);
            }

            try
            {
                Console.WriteLine("Run this application with argument '-h' or '--help' to get more info about the command line arguments.");
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
            string configPath = Path.Join(AssemblyHelper.GetCallingAssemblyRootPath(), "config.json");
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
            Console.WriteLine($"HTTP port: {port}");

            string defaultPfxPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "key.pfx");
            string pfxPath = argsDictionary.GetValue(Constants.ConfigKeys.PfxPathKey, defaultPfxPath);
            Console.WriteLine($"PFX path: {pfxPath}");

            string pfxPassword = argsDictionary.GetValue(Constants.ConfigKeys.PfxPasswordKey, "1234");
            Console.WriteLine($"PFX password: {pfxPassword}");

            int httpsPort = argsDictionary.GetValue(Constants.ConfigKeys.HttpsPortKey, 5050);
            Console.WriteLine($"HTTPS port: {httpsPort}");

            bool useHttps = argsDictionary.GetValue(Constants.ConfigKeys.UseHttpsKey, false);
            Console.WriteLine($"Use HTTPS: {useHttps}");

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