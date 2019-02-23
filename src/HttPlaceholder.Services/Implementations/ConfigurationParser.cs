using System;
using System.Collections.Generic;
using System.IO;
using Ducode.Essentials.Assembly.Interfaces;
using Ducode.Essentials.Console;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Utilities;
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
            if (argsDictionary.TryGetValue(Constants.ConfigKeys.ConfigJsonLocationKey, out string configJsonPath))
            {
                if (!_fileService.FileExists(configJsonPath))
                {
                    throw new FileNotFoundException($"File '{configJsonPath}' not found.");
                }

                ConsoleHelpers.WriteLineColor($"Reading configuration from '{configJsonPath}'.", ConsoleColor.Green, ConsoleColor.Black);
                string config = _fileService.ReadAllText(configJsonPath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
            }

            string configPath = Path.Combine(_assemblyService.GetCallingAssemblyRootPath(), "config.json");
            if (args.Length == 0 && _fileService.FileExists(configPath))
            {
                // If a config file is found, try to load and parse it instead of the arguments.
                Console.WriteLine($"Config file found at '{configPath}', so trying to parse that configuration.");

                ConsoleHelpers.WriteLineColor(
                    $"WARNING! Loading the configuration file from the installation folder is deprecated, because it can be overwritten when an update is installed. Use the --{Constants.ConfigKeys.ConfigJsonLocationKey} argument instead to provide the path to the configuration JSON file.",
                    ConsoleColor.Yellow,
                    ConsoleColor.Black);

                string config = _fileService.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
            }

            return argsDictionary;
        }
    }
}
