using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace HttPlaceholder.Utilities;

/// <summary>
/// TODO
/// </summary>
public static class HostBuilderUtilities
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder()
    {
        var builder = new HostBuilder();
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder
            .ConfigureLogging((hostingContext, logging) =>
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
