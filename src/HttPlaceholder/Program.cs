using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Configuration.Utilities;
using HttPlaceholder.Resources;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var version = AssemblyHelper.GetAssemblyVersion();
            HandleArgument(() => Console.WriteLine(version), args, new[] { "-v", "--version" });

            Console.WriteLine(ManPage.VersionHeader, version, DateTime.Now.Year);
            HandleArgument(() => Console.WriteLine(GetManPage()), args, new[] { "-h", "--help", "-?" });

            try
            {
                Console.WriteLine(ManPage.ExplanationHeader);
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(ManPage.ExceptionThrown, e);
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
            builder.AppendLine(ManPage.CmdExample);

            return builder.ToString();
        }

        private static string GetVerbosePage(IDictionary<string, string> args)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in args)
            {
                builder.AppendLine($"--{key}: {value}");
            }

            return builder.ToString();
        }

        private static void HandleArgument(Action action, IEnumerable<string> args, IEnumerable<string> argKeys, bool exit = true)
        {
            if (!args.Any(argKeys.Contains))
            {
                return;
            }

            action();
            if (exit)
            {
                Environment.Exit(0);
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
