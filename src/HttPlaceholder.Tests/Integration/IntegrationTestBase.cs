using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Configuration;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected IOptions<SettingsModel> Options = MockSettingsFactory.GetSettings();

        protected SettingsModel Settings => Options.Value;

        protected HttpClient Client;

        protected TestServer TestServer;

        protected void InitializeIntegrationTest((Type, object)[] servicesToReplace = null, IEnumerable<IStubSource> stubSources = null)
        {
            servicesToReplace = servicesToReplace ?? new (Type, object)[0];
            servicesToReplace = servicesToReplace.Concat(new (Type, object)[] { (typeof(IOptions<SettingsModel>), Options) }).ToArray();
            stubSources = stubSources ?? new IStubSource[0];
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Storage:InputFile", @"C:\tmp" }
                })
                .Build();
            var startup = new Startup(config);
            TestServer = new TestServer(new WebHostBuilder()
               .ConfigureServices(services => TestStartup.ConfigureServices(startup, services, servicesToReplace, stubSources))
               .Configure(app => TestStartup.Configure(startup, app, null)));
            Client = TestServer.CreateClient();
        }

        public void CleanupIntegrationTest() =>
           TestServer.Dispose();
    }
}
