using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Tests.Integration;

public static class TestStartup
{
    public static void ConfigureServices(Startup startup, IServiceCollection services,
        (Type, object)[] servicesToReplace, IEnumerable<IStubSource> stubSources)
    {
        startup.ConfigureServices(services);

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
        foreach (var (interfaceType, implementationType) in servicesToReplace)
        {
            services.AddTransient(interfaceType, _ => implementationType);
        }

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
            services.AddTransient(_ => service);
        }
    }

    public static void Configure(Startup startup, IApplicationBuilder app, IWebHostEnvironment env) =>
        startup.Configure(app, env);
}
