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
using Serilog;

namespace HttPlaceholder
{
    internal static class Program
    {
        private static readonly string[] _verboseArgs = {"-V", "--verbose"};
        private static readonly string[] _versionArgs = {"-v", "--version"};
        private static readonly string[] _helpArgs = {"-h", "--help", "-?"};

        public static int Main(string[] args)
        {
            var loggingConfig = new LoggerConfiguration();
            loggingConfig = args.Any(a => _verboseArgs.Contains(a))
                ? loggingConfig.MinimumLevel.Debug()
                : loggingConfig.MinimumLevel.Warning();
            Log.Logger = loggingConfig
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var version = AssemblyHelper.GetAssemblyVersion();
            HandleArgument(() => Console.WriteLine(version), args, _versionArgs);

            Console.WriteLine(ManPage.VersionHeader, version, DateTime.Now.Year);
            HandleArgument(() => Console.WriteLine(GetManPage()), args, _helpArgs);

            try
            {
                Console.WriteLine(ManPage.ExplanationHeader);
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var configParser = new ConfigurationParser();
            var argsDictionary = configParser.ParseConfiguration(args);
            var settings = DeserializeSettings(argsDictionary);

            HandleArgument(() => Console.WriteLine(GetVerbosePage(argsDictionary, args)), args, _verboseArgs,
                false);

            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(argsDictionary))
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    var httpPorts = ParsePorts(settings.Web.HttpPort);
                    foreach (var port in httpPorts)
                    {
                        options.Listen(IPAddress.Any, port);
                    }

                    if (settings.Web.UseHttps && !string.IsNullOrWhiteSpace(settings.Web.PfxPath) &&
                        !string.IsNullOrWhiteSpace(settings.Web.PfxPassword))
                    {
                        var httpsPorts = ParsePorts(settings.Web.HttpsPort);
                        foreach (var port in httpsPorts)
                        {
                            options.Listen(IPAddress.Any, port,
                                listenOptions =>
                                    listenOptions.UseHttps(settings.Web.PfxPath, settings.Web.PfxPassword));
                        }
                    }
                })
                .UseIIS()
                .Build();
        }

        private static int[] ParsePorts(string input)
        {
            var result = new List<int>();
            var httpPorts = input.Split(',').Select(p => p.Trim());
            foreach (var port in httpPorts)
            {
                if (!int.TryParse(port, out var parsedPort) || parsedPort < 1 || parsedPort > 65535)
                {
                    throw new ArgumentException($"Port '{port}' is invalid.");
                }

                result.Add(parsedPort);
            }

            return result.ToArray();
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

        private static string GetVerbosePage(IDictionary<string, string> argsDictionary, string[] args)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Provided command line arguments: {string.Join(" ", args)}");
            builder.AppendLine("Configuration that will be used by HttPlaceholder:");
            foreach (var (key, value) in argsDictionary)
            {
                builder.AppendLine($"--{key}: {value}");
            }

            return builder.ToString();
        }

        private static void HandleArgument(Action action, IEnumerable<string> args, IEnumerable<string> argKeys,
            bool exit = true)
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
