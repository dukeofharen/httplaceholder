using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Provider;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Infrastructure.Configuration;
using HttPlaceholder.Web.Shared.Resources;
using HttPlaceholder.Web.Shared.Utilities.Implementations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

namespace HttPlaceholder.Web.Shared.Utilities;

/// <summary>
///     A utility class for handling the starting of HttPlaceholder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ProgramUtilities
{
    private static readonly Stopwatch _startupWatch = new();
    private static readonly string[] _verboseArgs = {"-V", "--verbose"};
    private static readonly string[] _versionArgs = {"-v", "--version"};
    private static readonly string[] _helpArgs = {"-h", "--help", "-?"};

    /// <summary>
    ///     Configure the logging.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void ConfigureLogging(string[] args)
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
    public static void HandleCommands(string[] args)
    {
        var version = AssemblyHelper.GetAssemblyVersion();
        HandleArgument(() => Console.WriteLine(version), args, _versionArgs);

        Console.WriteLine(StringResources.VersionHeader, version, DateTime.Now.Year);
        HandleArgument(() => Console.WriteLine(GetManPage()), args, _helpArgs);
    }

    /// <summary>
    ///     Builds the web host for starting the application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The web host.</returns>
    public static IHost BuildWebHost<TStartup>(string[] args) where TStartup : class
    {
        var configParser = new ConfigurationParser();
        var argsDictionary = configParser.ParseConfiguration(args);
        var settings = DeserializeSettings(argsDictionary);

        if (IsVerbose(args))
        {
            Console.WriteLine(GetVerbosePage(argsDictionary, args));
        }

        return HostBuilderUtilities.CreateHostBuilder()
            .UseSerilog()
            .ConfigureAppConfiguration((_, config) => config.AddCustomInMemoryCollection(argsDictionary))
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseStartup<TStartup>()
                .UseKestrel(options => ConfigureKestrel(options, settings))
                .UseIIS())
            .Build();
    }

    /// <summary>
    ///     Start the startup timer.
    /// </summary>
    public static void StartStartupTimer() => _startupWatch.Start();

    /// <summary>
    ///     Get the amount of milliseconds the application took to start.
    /// </summary>
    /// <returns>The amount of milliseconds the application took to start.</returns>
    public static long GetStartupMillis()
    {
        _startupWatch.Stop();
        return _startupWatch.ElapsedMilliseconds;
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
        IEnumerable<string> argKeys)
    {
        if (!args.Any(argKeys.Contains))
        {
            return;
        }

        action();
        Environment.Exit(0);
    }

    private static SettingsModel DeserializeSettings(IDictionary<string, string> args)
    {
        var builder = new ConfigurationBuilder().AddCustomInMemoryCollection(args);
        var config = builder.Build();
        return config.Get<SettingsModel>();
    }

    private static void ConfigureKestrel(KestrelServerOptions options, SettingsModel settings)
    {
        var utility = new ProgramUtility();
        options.AddServerHeader = false;
        var (httpPorts, httpsPorts) = utility.GetPorts(settings);
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
        builder.AppendLine(StringResources.ExplanationHeader);
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

        builder.AppendLine(StringResources.CmdExample);

        return builder.ToString();
    }

    private static bool IsVerbose(IEnumerable<string> args)
    {
        var env = Environment.GetEnvironmentVariable("verbose");
        return args.Any(_verboseArgs.Contains) ||
               string.Equals(env, "true", StringComparison.OrdinalIgnoreCase);
    }
}
