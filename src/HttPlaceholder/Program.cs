using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HttPlaceholder.Resources;
using Microsoft.Extensions.Hosting;
using Serilog;
using static HttPlaceholder.Utilities.ProgramUtilities;

namespace HttPlaceholder;

[ExcludeFromCodeCoverage]
internal static class Program
{
    internal static Stopwatch StartupWatch = new();

    public static int Main(string[] args)
    {
        StartupWatch.Start();
        ConfigureLogging(args);
        HandleCommands(args);
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

    public static long GetStartupMillis()
    {
        StartupWatch.Stop();
        return StartupWatch.ElapsedMilliseconds;
    }
}
