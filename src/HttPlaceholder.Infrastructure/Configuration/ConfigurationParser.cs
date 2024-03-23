using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Implementations;
using Newtonsoft.Json;
using static HttPlaceholder.Domain.DefaultConfiguration;

namespace HttPlaceholder.Infrastructure.Configuration;

/// <summary>
///     A class that is used to convert a list of command line arguments to a dictionary.
/// </summary>
public class ConfigurationParser(
    IEnvService envService,
    IFileService fileService)
{
    /// <summary>
    ///     Constructs a <see cref="ConfigurationParser" /> instance.
    /// </summary>
    public ConfigurationParser() : this(
        new EnvService(),
        new FileService())
    {
    }

    /// <summary>
    ///     Converts a string array of command line arguments into a dictionary.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The parsed dictionary.</returns>
    public IDictionary<string, string> ParseConfiguration(IEnumerable<string> args)
    {
        var configMetadata = ConfigKeys.GetConfigMetadata().ToArray();
        var envResult = ParseEnvironment(configMetadata);
        var argsResult = args.Parse();
        var configFileResult = ParseConfigFile(envResult, argsResult);
        return DetermineFinalConfigDictionary(envResult, argsResult, configFileResult, configMetadata, args);
    }

    private IDictionary<string, string> ParseEnvironment(
        IEnumerable<ConfigMetadataModel> configMetadata)
    {
        var result = new Dictionary<string, string>();
        var envVars = envService.GetEnvironmentVariables();
        if (envVars == null)
        {
            return new Dictionary<string, string>();
        }

        foreach (var constant in configMetadata)
        {
            // Add the environment variables to the configuration.
            var envVar = envVars.CaseInsensitiveSearch(constant.Key);
            if (!string.IsNullOrWhiteSpace(envVar))
            {
                result.Add(constant.Key, envVar);
            }
        }

        return result;
    }

    private IDictionary<string, string> ParseConfigFile(
        IDictionary<string, string> envResult,
        IDictionary<string, string> argsResult)
    {
        const string key = ConfigKeys.ConfigJsonLocationKey;
        var configJsonPath = argsResult.CaseInsensitiveSearch(key);
        if (string.IsNullOrWhiteSpace(configJsonPath))
        {
            configJsonPath = envResult.CaseInsensitiveSearch(key);
        }

        if (string.IsNullOrWhiteSpace(configJsonPath))
        {
            return new Dictionary<string, string>();
        }

        // Read the settings from a given file if the correct config key is set.
        if (!fileService.FileExists(configJsonPath))
        {
            throw new FileNotFoundException($"File '{configJsonPath}' not found.");
        }

        Console.WriteLine($"Reading configuration from '{configJsonPath}'.");
        var config = fileService.ReadAllText(configJsonPath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
    }

    private IDictionary<string, string> DetermineFinalConfigDictionary(
        IDictionary<string, string> envResult,
        IDictionary<string, string> argsResult,
        IDictionary<string, string> configFileResult,
        ConfigMetadataModel[] configMetadata,
        IEnumerable<string> args)
    {
        var result = new Dictionary<string, string>();
        foreach (var constant in configMetadata)
        {
            var valueFound = false;
            if (argsResult.TryGetCaseInsensitive(constant.Key, out var value))
            {
                valueFound = true;
            }
            else if (envResult.TryGetCaseInsensitive(constant.Key, out value))
            {
                valueFound = true;
            }
            else if (configFileResult.TryGetCaseInsensitive(constant.Key, out value))
            {
                valueFound = true;
            }

            if (!valueFound)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(value) && constant.IsBoolValue == true)
            {
                // The property is a boolean and no value was provided. Interpret this as "true".
                value = "true";
            }

            result.Add(constant.Key.ToLower(), value);
        }

        EnsureDefaultValuesAreAdded(result);
        var finalResult = new Dictionary<string, string>();
        foreach (var item in result)
        {
            var constant =
                configMetadata.FirstOrDefault(m => string.Equals(item.Key, m.Key, StringComparison.OrdinalIgnoreCase));
            if (constant != null && !string.IsNullOrWhiteSpace(constant.Path))
            {
                finalResult.Add(constant.Path, item.Value);
            }
        }

        if (CliArgs.IsVerbose(args))
        {
            finalResult.Add("Logging:VerboseLoggingEnabled", "True");
        }

        return finalResult;
    }

    private void EnsureDefaultValuesAreAdded(IDictionary<string, string> configDictionary)
    {
        configDictionary.EnsureEntryExists(ConfigKeys.PortKey, DefaultHttpPort);
        configDictionary.EnsureEntryExists(ConfigKeys.PfxPathKey,
            Path.Combine(AssemblyHelper.GetEntryAssemblyRootPath(), DefaultPfxPath));
        configDictionary.EnsureEntryExists(ConfigKeys.PfxPasswordKey, DefaultPfxPassword);
        configDictionary.EnsureEntryExists(ConfigKeys.HttpsPortKey, DefaultHttpsPort);
        configDictionary.EnsureEntryExists(ConfigKeys.UseHttpsKey, UseHttps);
        configDictionary.EnsureEntryExists(ConfigKeys.EnableUserInterface, EnableUserInterface);
        configDictionary.EnsureEntryExists(ConfigKeys.OldRequestsQueueLengthKey, DefaultOldRequestsQueueLength);
        configDictionary.EnsureEntryExists(ConfigKeys.MaximumExtraDurationMillisKey, DefaultMaximumExtraDuration);
        configDictionary.EnsureEntryExists(ConfigKeys.CleanOldRequestsInBackgroundJob, CleanOldRequestsInBackgroundJob);
        configDictionary.EnsureEntryExists(ConfigKeys.StoreResponses, StoreResponses);
        configDictionary.EnsureEntryExists(ConfigKeys.ReadProxyHeaders, ReadProxyHeaders);
        configDictionary.EnsureEntryExists(ConfigKeys.AllowGlobalFileSearch, AllowGlobalFileSearch);

        // Determine and set file storage location.
        SetDefaultFileStorageLocation(configDictionary);
    }

    private void SetDefaultFileStorageLocation(IDictionary<string, string> argsDictionary)
    {
        if (argsDictionary.ContainsKeyCaseInsensitive(ConfigKeys.FileStorageLocationKey))
        {
            return;
        }

        string fileStorageLocation = null;
        var windowsProfilePath = envService.GetEnvironmentVariable("USERPROFILE");
        var unixProfilePath = envService.GetEnvironmentVariable("HOME");
        const string stubFolderName = ".httplaceholder";
        if (envService.IsOs(OSPlatform.Windows) && fileService.DirectoryExists(windowsProfilePath))
        {
            fileStorageLocation = $"{windowsProfilePath}\\{stubFolderName}";
        }
        else if (
            (envService.IsOs(OSPlatform.Linux) ||
             envService.IsOs(OSPlatform.OSX)) && fileService.DirectoryExists(unixProfilePath))
        {
            fileStorageLocation = $"{unixProfilePath}/{stubFolderName}";
        }

        if (!string.IsNullOrWhiteSpace(fileStorageLocation))
        {
            argsDictionary.EnsureEntryExists(ConfigKeys.FileStorageLocationKey, fileStorageLocation);
        }
    }
}
