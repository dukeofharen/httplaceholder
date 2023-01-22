using System;
using System.Diagnostics.CodeAnalysis;
using HttPlaceholder.Web.Shared.Resources;
using HttPlaceholder.Web.Shared.Utilities;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HttPlaceholder;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static int Main(string[] args)
    {
        ProgramUtilities.StartStartupTimer();
        ProgramUtilities.ConfigureLogging(args);
        ProgramUtilities.HandleCommands(args);
        try
        {
            Console.WriteLine(StringResources.ExplanationHeader);
            ProgramUtilities.BuildWebHost<Startup>(args).Run();
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
}
