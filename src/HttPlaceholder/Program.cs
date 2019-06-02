using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Console;
using HttPlaceholder.Configuration;
using HttPlaceholder.Configuration.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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

            int port = argsDictionary.GetAndSetValue(ConfigKeys.PortKey, 5000);
            string pfxPath = argsDictionary.GetAndSetValue(ConfigKeys.PfxPathKey, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "key.pfx"));
            string pfxPassword = argsDictionary.GetAndSetValue(ConfigKeys.PfxPasswordKey, "1234");
            int httpsPort = argsDictionary.GetAndSetValue(ConfigKeys.HttpsPortKey, 5050);
            bool useHttps = argsDictionary.GetAndSetValue(ConfigKeys.UseHttpsKey, false);
            HandleArgument(() => Console.WriteLine(GetVerbosePage(argsDictionary)), args, new string[] { "-V", "--verbose" }, false);

            return WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((_, config) => config.AddHttPlaceholderConfiguration(argsDictionary))
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
            foreach (var constant in ConfigurationUtilities.GetConfigKeyMetadata())
            {
                builder.AppendLine($"--{constant.configKey}: {constant.description} (e.g. {constant.example})");
            }

            builder.AppendLine();
            builder.AppendLine("Example: httplaceholder --apiUsername user --apiPassword pass");

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
    }
}
