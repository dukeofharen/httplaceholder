using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Placeholder.Implementation;
using Placeholder.Middleware;

namespace Placeholder
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMvc();
         services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         DependencyRegistration.RegisterDependencies(services);
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         app.UseMiddleware<StubHandlingMiddleware>();
         app.UseMvc();

         // Check if the stubs can be loaded.
         var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
         stubContainer.GetStubs();
      }
   }
}
