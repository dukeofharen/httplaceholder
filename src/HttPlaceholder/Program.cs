using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Configuration.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            string version = AssemblyHelper.GetAssemblyVersion();
            HandleArgument(() => Console.WriteLine(version), args, new string[] { "-v", "--version" });

            Console.WriteLine($"HttPlaceholder {version} - (c) {DateTime.Now.Year} Ducode");
            HandleArgument(() => Console.WriteLine(GetManPage()), args, new string[] { "-h", "--help", "-?" });

            try
            {
                Console.WriteLine("Run this application with argument '-h' or '--help' to get more info about the command line arguments.");
                Console.WriteLine("When running in to trouble, or just see what's going on, run this application with argument '-V' or '--verbose' to print the configuration variables.");
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception thrown: {e}");
                Environment.Exit(-1);
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var configParser = new ConfigurationParser();
            var argsDictionary = configParser.ParseConfiguration(args);
            var settings = DeserializeSettings(argsDictionary);

            HandleArgument(() => Console.WriteLine(GetVerbosePage(argsDictionary)), args, new [] { "-V", "--verbose" }, false);

            return WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(argsDictionary))
               .UseStartup<Startup>()
               .UseKestrel(options =>
               {
                   options.AddServerHeader = false;
                   options.Listen(IPAddress.Any, settings.Web.HttpPort);
                   if (settings.Web.UseHttps && !string.IsNullOrWhiteSpace(settings.Web.PfxPath) && !string.IsNullOrWhiteSpace(settings.Web.PfxPassword))
                   {
                       options.Listen(IPAddress.Any, settings.Web.HttpsPort, listenOptions => listenOptions.UseHttps(settings.Web.PfxPath, settings.Web.PfxPassword));
                   }
               })
               .Build();
        }

        private static string GetManPage()
        {
            var builder = new StringBuilder();
            foreach (var constant in ConfigurationParser.GetConfigKeyMetadata())
            {
                builder.AppendLine($"--{constant.Key}: {constant.Description} (e.g. {constant.Example})");
            }

            builder.AppendLine();
            builder.AppendLine("Example: httplaceholder --apiusername user --apipassword pass");

            return builder.ToString();
        }

        private static string GetVerbosePage(IDictionary<string, string> args)
        {
            var builder = new StringBuilder();
            foreach (var pair in args)
            {
                builder.AppendLine($"--{pair.Key}: {pair.Value}");
            }

            return builder.ToString();
        }

        private static void HandleArgument(Action action, string[] args, string[] argKeys, bool exit = true)
        {
            if (args.Any(arg => argKeys.Contains(arg)))
            {
                action();
                if (exit)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static SettingsModel DeserializeSettings(IDictionary<string, string> args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(args);
            var config = builder.Build();
            return config.Get<SettingsModel>();
        }
    }
}
