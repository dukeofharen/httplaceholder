using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Tests.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Integration
{
   public static class TestStartup
   {
      public static void ConfigureServices(Startup startup, IServiceCollection services, (Type, object)[] servicesToReplace, IEnumerable<IStubSource> stubSources)
      {
         // Delete old services
         var servicesToDelete = servicesToReplace
            .Select(str => str.Item1)
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
            services.AddTransient(service.Item1, serviceProvider => service.Item2);
         }

         var loggerFactoryMock = new Mock<ILoggerFactory>();
         var logger = new MockLogger();

         loggerFactoryMock
            .Setup(m => m.CreateLogger(It.IsAny<string>()))
            .Returns(logger);

         services.Add(new ServiceDescriptor(typeof(ILoggerFactory), loggerFactoryMock.Object));
         startup.ConfigureServices(services);

         // Replace all stub sources. The tests should prepare their own stub sources.
         serviceDescriptors = services
            .Where(s => s.ServiceType == typeof(IStubSource))
            .ToArray();
         foreach (var descriptor in serviceDescriptors)
         {
            services.Remove(descriptor);
         }

         foreach (var service in stubSources)
         {
            services.AddTransient(provider => service);
         }
      }

      public static void Configure(Startup startup, IApplicationBuilder app, IHostingEnvironment env)
      {
         startup.Configure(app, env);
      }
   }
}
