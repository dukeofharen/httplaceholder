using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected readonly IOptions<SettingsModel> Options = MockSettingsFactory.GetOptions();

    protected SettingsModel Settings => Options.Value;

    protected HttpClient Client;

    protected TestServer TestServer;

    protected string BaseAddress => TestServer.BaseAddress.ToString();

    protected void InitializeIntegrationTest((Type, object)[] servicesToReplace = null, IEnumerable<IStubSource> stubSources = null)
    {
        servicesToReplace ??= Array.Empty<(Type, object)>();
        servicesToReplace = servicesToReplace.Concat(new (Type, object)[] { (typeof(IOptions<SettingsModel>), Options) }).ToArray();
        stubSources ??= Array.Empty<IStubSource>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Storage:InputFile", @"C:\tmp" },
                { "Storage:CleanOldRequestsInBackgroundJob", "true" }
            })
            .Build();
        var startup = new Startup(config);
        TestServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services => TestStartup.ConfigureServices(startup, services, servicesToReplace, stubSources))
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
