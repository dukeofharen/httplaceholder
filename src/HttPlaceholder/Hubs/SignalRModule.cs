using HttPlaceholder.Hubs.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Hubs
{
    public static class SignalRModule
    {
        public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddTransient<IRequestNotify, RequestNotify>();
            return services;
        }

        public static IApplicationBuilder UseSignalRHubs(this IApplicationBuilder app)
        {
            app.UseSignalR(r => r.MapHub<RequestHub>("/requestHub"));
            return app;
        }
    }
}
