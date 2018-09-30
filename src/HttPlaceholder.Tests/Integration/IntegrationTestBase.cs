using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace HttPlaceholder.Tests.Integration
{
   public abstract class IntegrationTestBase
   {
      protected HttpClient Client;

      protected TestServer TestServer;

      protected void InitializeIntegrationTest((Type, object)[] servicesToReplace = null)
      {
         servicesToReplace = servicesToReplace ?? new (Type, object)[0];
         var startup = new Startup();
         TestServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services => TestStartup.ConfigureServices(startup, services, servicesToReplace))
            .Configure(app => TestStartup.Configure(startup, app, null)));
         Client = TestServer.CreateClient();
      }

      public void CleanupIntegrationTest()
      {
         TestServer.Dispose();
      }
   }
}
