using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.SwaggerGenerator.Tools;

namespace HttPlaceholder.SwaggerGenerator
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var tools = new[] {new SwaggerGen()};
            var toolKeys = tools.Select(t => t.Key).ToArray();
            var toolKeyHelpString = string.Join(", ", toolKeys);
            if (!args.Any())
            {
                Console.WriteLine($"No arguments provided. Available arguments: {toolKeyHelpString}");
                Environment.Exit(-1);
            }

            var toolKey = args[0];
            if (!toolKeys.Any(k => k.Equals(toolKey, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Tool {toolKey} not found. Available arguments: {toolKeyHelpString}");
                Environment.Exit(-1);
            }

            var tool = tools.Single(t => t.Key.Equals(toolKey, StringComparison.OrdinalIgnoreCase));
            await tool.ExecuteAsync(args.Skip(1).ToArray());
        }
    }
}
