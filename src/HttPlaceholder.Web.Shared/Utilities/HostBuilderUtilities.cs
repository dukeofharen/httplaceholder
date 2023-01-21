using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.EventLog;

namespace HttPlaceholder.Web.Shared.Utilities;

/// <summary>
///     A class that contains methods for creating a host builder.
/// </summary>
public static class HostBuilderUtilities
{
    /// <summary>
    ///     Creates a <see cref="IHostBuilder" /> specifically for HttPlaceholder.
    ///     One of the problems we had is that when using the default host builder,
    ///     .NET creates a lot of unnecessary Inotify watches.
    /// </summary>
    public static IHostBuilder CreateHostBuilder()
    {
        var builder = new HostBuilder();
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder
            .ConfigureLogging((_, logging) =>
            {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {
                    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                }

                logging.AddDebug();
                logging.AddEventSourceLogger();
                if (isWindows)
                {
                    logging.AddEventLog();
                }
            })
            .UseDefaultServiceProvider((context, options) =>
            {
                var isDevelopment = context.HostingEnvironment.IsDevelopment();
                options.ValidateScopes = isDevelopment;
                options.ValidateOnBuild = isDevelopment;
            });
        return builder;
    }
}
