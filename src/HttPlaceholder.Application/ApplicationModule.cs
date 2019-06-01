using AutoMapper;
using FluentValidation;
using HttPlaceholder.Application.Infrastructure.MediatR;
using HttPlaceholder.Application.StubExecution;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationModule(this IServiceCollection services)
        {
            var currentAssembly = typeof(ApplicationModule).Assembly;

            // Add MediatR
            services.AddMediatR(currentAssembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // Add fluent validations
            services.AddValidatorsFromAssemblies(new[] { currentAssembly });

            // Add other modules
            services.AddStubExecutionModule();

            return services;
        }
    }
}
