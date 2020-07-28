using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration.Attributes;
using HttPlaceholder.Infrastructure.Implementations;
using Newtonsoft.Json;

namespace HttPlaceholder.Configuration.Utilities
{
    public class ConfigurationParser
    {
        private readonly IEnvService _envService;
        private readonly IFileService _fileService;

        public ConfigurationParser(
            IEnvService envService,
            IFileService fileService)
        {
            _envService = envService;
            _fileService = fileService;
        }

        public ConfigurationParser() : this(
            new EnvService(),
            new FileService())
        {
        }

        public IDictionary<string, string> ParseConfiguration(string[] args)
        {
            var argsDictionary = DetermineBaseArgsDictionary(args);
            argsDictionary = argsDictionary.ToDictionary(d => d.Key.ToLower(), d => d.Value);
            EnsureDefaultValuesAreAdded(argsDictionary);
            return BuildFinalArgsDictionary(argsDictionary);
        }

        private IDictionary<string, string> DetermineBaseArgsDictionary(IEnumerable<string> args)
        {
            var argsDictionary = args.Parse();
            var key = ConfigKeys.ConfigJsonLocationKey.ToLower();
            if (!argsDictionary.TryGetValue(key, out var configJsonPath))
            {
                var envVars = _envService.GetEnvironmentVariables();
                configJsonPath = envVars?.FirstOrDefault(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    .Value;
                if (string.IsNullOrWhiteSpace(configJsonPath))
                {
                    return argsDictionary;
                }
            }

            // Read the settings from a given file if the correct config key is set
            if (!_fileService.FileExists(configJsonPath))
            {
                throw new FileNotFoundException($"File '{configJsonPath}' not found.");
            }

            ConsoleHelpers.WriteLineColor($"Reading configuration from '{configJsonPath}'.", ConsoleColor.Green,
                ConsoleColor.Black);
            var config = _fileService.ReadAllText(configJsonPath);
            argsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);

            return argsDictionary;
        }

        private static void EnsureDefaultValuesAreAdded(IDictionary<string, string> argsDictionary)
        {
            argsDictionary.EnsureEntryExists(ConfigKeys.PortKey, 5000);
            argsDictionary.EnsureEntryExists(ConfigKeys.PfxPathKey,
                Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "key.pfx"));
            argsDictionary.EnsureEntryExists(ConfigKeys.PfxPasswordKey, "1234");
            argsDictionary.EnsureEntryExists(ConfigKeys.HttpsPortKey, 5050);
            argsDictionary.EnsureEntryExists(ConfigKeys.UseHttpsKey, true);
            argsDictionary.EnsureEntryExists(ConfigKeys.EnableUserInterface, true);
            argsDictionary.EnsureEntryExists(ConfigKeys.OldRequestsQueueLengthKey, 40);
        }

        private IDictionary<string, string> BuildFinalArgsDictionary(IDictionary<string, string> argsDictionary)
        {
            var envVars = _envService.GetEnvironmentVariables();
            envVars = envVars?.ToDictionary(d => d.Key.ToLower(), d => d.Value);

            var configDictionary = new Dictionary<string, string>();
            foreach (var constant in GetConfigKeyMetadata())
            {
                if (string.IsNullOrWhiteSpace(constant.Path))
                {
                    continue;
                }

                // First, add the environment variables to the configuration.
                if (envVars != null && envVars.TryGetValue(constant.Key.ToLower(), out var envVar))
                {
                    configDictionary.Add(constant.Path, envVar);
                    continue;
                }

                // Then, add the default values and values passed through the command line arguments.
                if (argsDictionary.TryGetValue(constant.Key, out var value))
                {
                    if (constant.IsBoolValue == true && string.IsNullOrWhiteSpace(value))
                    {
                        // The property is a boolean and no value was provided. Interpret this as "true".
                        value = "True";
                    }

                    configDictionary.Add(constant.Path, value);
                }
            }

            return configDictionary;
        }

        public static IEnumerable<ConfigMetadataModel> GetConfigKeyMetadata() =>
            (from constant
                    in ReflectionUtilities.GetConstants(typeof(ConfigKeys))
                let attribute = constant.CustomAttributes.FirstOrDefault()
                where attribute != null && attribute.AttributeType == typeof(ConfigKeyAttribute)
                select new ConfigMetadataModel
                {
                    Key = (constant.GetValue(constant) as string)?.ToLower(),
                    Description = ParseAttribute<string>(attribute, "Description", true),
                    Example = ParseAttribute<string>(attribute, "Example", true),
                    Path = ParseAttribute<string>(attribute, "ConfigPath", false),
                    IsBoolValue = ParseAttribute<bool?>(attribute, "IsBoolValue", false)
                }).ToList();

        private static TValue ParseAttribute<TValue>(CustomAttributeData attribute, string memberName, bool shouldExist)
        {
            if (attribute.NamedArguments == null)
            {
                throw new InvalidOperationException("NamedArguments not set.");
            }

            var result =
                attribute.NamedArguments.FirstOrDefault(a => a.MemberName == memberName).TypedValue.Value;
            if (shouldExist && result == null)
            {
                throw new InvalidOperationException(
                    $"Property with name '{memberName}' was not found, but it should exist.");
            }

            return (TValue)result;
        }
    }
}
