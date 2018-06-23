using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Placeholder.Swagger
{
   public class StatusCodeOperationFilter : IOperationFilter
   {
      public void Apply(Operation operation, OperationFilterContext context)
      {
         operation.Responses.Add("401", new Response
         {
            Description = "User is not authorized (if authorization is configured)"
         });
      }
   }
}
