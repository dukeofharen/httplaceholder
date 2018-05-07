using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Placeholder.Tests.Integration
{
   public abstract class IntegrationTestBase
   {
      protected HttpClient Client;

      protected TestServer TestServer;

      protected void InitializeIntegrationTest(Dictionary<Type, object> extraServicesToReplace = null)
      {
         var servicesToReplace = new Dictionary<Type, object>();
         if (extraServicesToReplace != null)
         {
            foreach (var extraService in extraServicesToReplace)
            {
               servicesToReplace.Add(extraService.Key, extraService.Value);
            }
         }

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
