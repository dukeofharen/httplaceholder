using System;
using System.Collections.Generic;
using System.IO;
using Ducode.Essentials.Assembly.Interfaces;
using Ducode.Essentials.Console;
using Ducode.Essentials.Files.Interfaces;
using Newtonsoft.Json;

namespace HttPlaceholder.Services.Implementations
{
    public class ConfigurationParser : IConfigurationParser
    {
        private readonly IAssemblyService _assemblyService;
        private readonly IFileService _fileService;

        public ConfigurationParser(
            IAssemblyService assemblyService,
            IFileService fileService)
        {
            _assemblyService = assemblyService;
            _fileService = fileService;
        }

        public IDictionary<string, string> ParseConfiguration(string[] args)
        {
            var argsDictionary = args.Parse();
            string configPath = Path.Combine(_assemblyService.GetCallingAssemblyRootPath(), "config.json");
            if (args.Length == 0 && File.Exists(configPath))
            {
                // If a config file is found, try to load and parse it instead of the arguments.
                Console.WriteLine($"Config file found at '{configPath}', so trying to parse that configuration.");
                string config = _fileService.ReadAllText(configPath);
                argsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
            }

            return argsDictionary;
        }
    }
}
