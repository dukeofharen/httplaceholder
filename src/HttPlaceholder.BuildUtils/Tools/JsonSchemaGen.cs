using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries.GetJsonSchema;
using HttPlaceholder.Common.Utilities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.SwaggerGenerator.Tools
{
    public class JsonSchemaGen : ITool
    {
        public string Key => "jsonschemagen";

        public async Task ExecuteAsync(string[] args)
        {
            var rootPath = AssemblyHelper.GetCallingAssemblyRootPath();
            if (args.Any())
            {
                rootPath = args[0];
                if (!Directory.Exists(rootPath))
                {
                    throw new ArgumentException($"Path '{rootPath}' doesn't exist.");
                }
            }

            var services = new ServiceCollection();
            Startup.ConfigureServicesStatic(services, new ConfigurationBuilder().Build());
            var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var schemaWithoutArray = await mediator.Send(new GetJsonSchemaQuery(false));
            var schemaWithArray = await mediator.Send(new GetJsonSchemaQuery(true));

            var schemaWithoutArrayPath = Path.Join(rootPath, "schema.json");
            Console.WriteLine($"Writing file {schemaWithoutArrayPath}");
            await File.WriteAllTextAsync(schemaWithoutArrayPath, schemaWithoutArray);

            var schemaWithArrayPath = Path.Join(rootPath, "schema_array.json");
            Console.WriteLine($"Writing file {schemaWithArrayPath}");
            await File.WriteAllTextAsync(schemaWithArrayPath, schemaWithArray);
        }
    }
}
