using System;
using HttPlaceholder;
using HttPlaceholder.Web.Shared.Utilities;
using Microsoft.Extensions.Hosting;
using Serilog;

ProgramUtilities.StartStartupTimer();
ProgramUtilities.ConfigureLogging(args);
ProgramUtilities.HandleCommands(args);

try
{
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
