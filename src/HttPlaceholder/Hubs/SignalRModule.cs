using HttPlaceholder.Application.Infrastructure.Newtonsoft;
using HttPlaceholder.Hubs.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Hubs
{
    public static class SignalRModule
    {
        public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
        {
            services
                .AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                    options.PayloadSerializerSettings.ContractResolver = new CamelCaseExceptDictionaryKeysResolver());
            services.AddTransient<IRequestNotify, RequestNotify>();
            return services;
        }
    }
}
