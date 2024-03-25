using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected readonly IOptionsMonitor<SettingsModel> Options = MockSettingsFactory.GetOptionsMonitor();

    protected HttpClient Client;

    protected TestServer TestServer;

    protected SettingsModel Settings => Options.CurrentValue;

    protected string BaseAddress => TestServer.BaseAddress.ToString();

    protected void InitializeIntegrationTest((Type, object)[] servicesToReplace = null,
        IEnumerable<IStubSource> stubSources = null)
    {
        servicesToReplace ??= Array.Empty<(Type, object)>();
        stubSources ??= Array.Empty<IStubSource>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Storage:InputFile", @"C:\tmp" },
                { "Storage:CleanOldRequestsInBackgroundJob", "true" },
                { "Gui:EnableUserInterface", "true" }
            })
            .Build();
        servicesToReplace = servicesToReplace.Concat(new (Type, object)[]
        {
            (typeof(IOptionsMonitor<SettingsModel>), Options), (typeof(IConfiguration), config)
        }).ToArray();
        var startup = new Startup(config);
        TestServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
                TestStartup.ConfigureServices(startup, services, servicesToReplace, stubSources))
            .Configure(app => TestStartup.Configure(startup, app, null)));
        Client = TestServer.CreateClient();
        AfterTestServerStart();
    }

    protected void CleanupIntegrationTest() =>
        TestServer.Dispose();

    protected virtual void AfterTestServerStart()
    {
    }
}
