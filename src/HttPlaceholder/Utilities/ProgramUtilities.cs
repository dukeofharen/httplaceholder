using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Provider;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Configuration;
using HttPlaceholder.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HttPlaceholder.Utilities;

/// <summary>
///     A utility class for handling the starting of HttPlaceholder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ProgramUtilities
{
    private static readonly string[] _verboseArgs = {"-V", "--verbose"};
    private static readonly string[] _versionArgs = {"-v", "--version"};
    private static readonly string[] _helpArgs = {"-h", "--help", "-?"};

    /// <summary>
    ///     Configure the logging.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void ConfigureLogging(IEnumerable<string> args)
    {
        var verbose = IsVerbose(args);
        var loggingConfig = new LoggerConfiguration();
        loggingConfig = verbose
            ? loggingConfig.MinimumLevel.Debug()
            : loggingConfig.MinimumLevel.Information();
        Log.Logger = loggingConfig
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    /// <summary>
    ///     Handle several commands.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void HandleCommands(IEnumerable<string> args)
    {
        var version = AssemblyHelper.GetAssemblyVersion();
        var argsArray = args as string[] ?? args.ToArray();
        HandleArgument(() => Console.WriteLine(version), argsArray, _versionArgs);

        Console.WriteLine(ManPage.VersionHeader, version, DateTime.Now.Year);
        HandleArgument(() => Console.WriteLine(GetManPage()), argsArray, _helpArgs);
    }

    /// <summary>
    ///     Builds the web host for starting the application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The web host.</returns>
    public static IHost BuildWebHost(string[] args)
    {
        var configParser = new ConfigurationParser();
        var argsDictionary = configParser.ParseConfiguration(args);
        var settings = DeserializeSettings(argsDictionary);

        if (IsVerbose(args))
        {
            Console.WriteLine(GetVerbosePage(argsDictionary, args));
        }

        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((_, config) => config.AddCustomInMemoryCollection(argsDictionary))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                    .UseKestrel(options => ConfigureKestrel(options, settings))
                    .UseIIS();
            })
            .Build();
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

    private static void HandleArgument(
        Action action,
        IEnumerable<string> args,
        IEnumerable<string> argKeys,
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
        var builder = new ConfigurationBuilder().AddCustomInMemoryCollection(args);
        var config = builder.Build();
        return config.Get<SettingsModel>();
    }

    private static void ConfigureKestrel(KestrelServerOptions options, SettingsModel settings)
    {
        options.AddServerHeader = false;
        var (httpPorts, httpsPorts) = GetPorts(settings);
        foreach (var port in httpPorts)
        {
            options.Listen(IPAddress.Any, port);
        }

        foreach (var port in httpsPorts)
        {
            options.Listen(IPAddress.Any, port,
                listenOptions =>
                    listenOptions.UseHttps(settings.Web.PfxPath, settings.Web.PfxPassword));
        }
    }

    private static string GetManPage()
    {
        var builder = new StringBuilder();
        builder.AppendLine(ManPage.ExplanationHeader);
        builder.AppendLine();

        var metadata = ConfigKeys.GetConfigMetadata();
        var configKeyTypes = ConfigKeys.GetConfigKeyTypes();
        var groupedMetadata = metadata
            .Select(m => (Type: configKeyTypes[m.ConfigKeyType], ConfigMetadataModel: m))
            .GroupBy(m => m.Type)
            .OrderBy(g => g.Key);
        foreach (var group in groupedMetadata)
        {
            builder.AppendLine($"{group.Key}:");
            foreach (var constant in group)
            {
                builder.AppendLine(
                    $"--{constant.ConfigMetadataModel.DisplayKey}: {constant.ConfigMetadataModel.Description} (e.g. {constant.ConfigMetadataModel.Example})");
            }

            builder.AppendLine();
        }

        builder.AppendLine(ManPage.CmdExample);

        return builder.ToString();
    }

    private static int[] ParsePorts(string input)
    {
        var result = new List<int>();
        var httpPorts = input.Split(',').Select(p => p.Trim());
        foreach (var port in httpPorts)
        {
            if (!int.TryParse(port, out var parsedPort) || parsedPort is < 1 or > 65535)
            {
                throw new ArgumentException($"Port '{port}' is invalid.");
            }

            result.Add(parsedPort);
        }

        return result.ToArray();
    }

    private static (IEnumerable<int> httpPorts, IEnumerable<int> httpsPorts) GetPorts(SettingsModel settings)
    {
        var httpPortsResult = new List<int>();
        var httpsPortsResult = new List<int>();

        var httpPorts = ParsePorts(settings.Web.HttpPort);
        if (httpPorts.Length == 1 && httpPorts[0] == DefaultConfiguration.DefaultHttpPort &&
            TcpUtilities.PortIsTaken(httpPorts[0]))
        {
            httpPortsResult.Add(TcpUtilities.GetNextFreeTcpPort());
        }
        else
        {
            httpPortsResult.AddRange(httpPorts);
        }

        if (settings.Web.UseHttps && !string.IsNullOrWhiteSpace(settings.Web.PfxPath) &&
            !string.IsNullOrWhiteSpace(settings.Web.PfxPassword))
        {
            var httpsPorts = ParsePorts(settings.Web.HttpsPort);
            if (httpsPorts.Length == 1 && httpsPorts[0] == DefaultConfiguration.DefaultHttpsPort &&
                TcpUtilities.PortIsTaken(httpsPorts[0]))
            {
                httpsPortsResult.Add(TcpUtilities.GetNextFreeTcpPort());
            }
            else
            {
                httpsPortsResult.AddRange(httpsPorts);
            }
        }

        return (httpPortsResult, httpsPortsResult);
    }

    private static bool IsVerbose(IEnumerable<string> args)
    {
        var env = Environment.GetEnvironmentVariable("verbose");
        return args.Any(_verboseArgs.Contains) ||
               string.Equals(env, "true", StringComparison.OrdinalIgnoreCase);
    }
}
