using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Console;
using Ducode.Essentials.Files;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Attributes;
using HttPlaceholder.Services.Implementations;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var arg = args.FirstOrDefault();
            string version = AssemblyHelper.GetAssemblyVersion();
            if (string.Equals(arg, "-v", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, "--version", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(version);
                Environment.Exit(0);
            }

            Console.WriteLine($"HttPlaceholder {version} - (c) 2019 Ducode");

            if (string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(GetManPage());
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
            var configParser = new ConfigurationParser(new AssemblyService(), new FileService());
            var argsDictionary = configParser.ParseConfiguration(args);

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

        private static string GetManPage()
        {
            var builder = new StringBuilder();
            var constants = ReflectionUtilities.GetConstants(typeof(Constants.ConfigKeys));
            foreach (var constant in constants)
            {
                var attribute = constant.CustomAttributes.FirstOrDefault();
                if (attribute != null && attribute.AttributeType == typeof(ConfigKeyAttribute))
                {
                    var value = constant.GetValue(constant);
                    string description = attribute.NamedArguments.Single(a => a.MemberName == "Description").TypedValue.Value as string;
                    string example = attribute.NamedArguments.Single(a => a.MemberName == "Example").TypedValue.Value as string;
                    builder.AppendLine($"--{value}: {description} (e.g. {example})");
                }
            }

            builder.AppendLine();
            builder.AppendLine("Example: httplaceholder --apiUsername user --apiPassword pass");

            return builder.ToString();
        }
    }
}