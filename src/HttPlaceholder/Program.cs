using System;
using HttPlaceholder.Resources;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using static HttPlaceholder.Utilities.ProgramUtilities;

namespace HttPlaceholder
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
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
    }
}
