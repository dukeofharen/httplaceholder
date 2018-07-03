using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using HttPlaceholder.Tests.Utilities;

namespace HttPlaceholder.Tests.Integration
{
   public static class TestStartup
   {
      public static void ConfigureServices(Startup startup, IServiceCollection services, IDictionary<Type, object> servicesToReplace)
      {
         startup.ConfigureServices(services);

         // Delete old services
         var servicesToDelete = servicesToReplace
            .Select(str => str.Key)
            .ToArray();
         var serviceDescriptors = services
            .Where(s => servicesToDelete.Contains(s.ServiceType))
            .ToArray();
         foreach (var descriptor in serviceDescriptors)
         {
            services.Remove(descriptor);
         }

         // Add mock services
         foreach (var service in servicesToReplace)
         {
            services.AddTransient(service.Key, serviceProvider => service.Value);
         }

         var loggerFactoryMock = new Mock<ILoggerFactory>();
         var logger = new MockLogger();

         loggerFactoryMock
            .Setup(m => m.CreateLogger(It.IsAny<string>()))
            .Returns(logger);

         services.Add(new ServiceDescriptor(typeof(ILoggerFactory), loggerFactoryMock.Object));
      }

      public static void Configure(Startup startup, IApplicationBuilder app, IHostingEnvironment env)
      {
         startup.Configure(app, env);
      }
   }
}
