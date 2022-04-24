using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Attributes;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Infrastructure.Implementations;
using Newtonsoft.Json;
using static HttPlaceholder.Domain.Constants;

namespace HttPlaceholder.Infrastructure.Configuration;

/// <summary>
/// A class that is used to convert a list of command line arguments to a dictionary.
/// </summary>
public class ConfigurationParser
{
    private readonly IEnvService _envService;
    private readonly IFileService _fileService;

    /// <summary>
    /// Constructs a <see cref="ConfigurationParser"/> instance.
    /// </summary>
    internal ConfigurationParser(
        IEnvService envService,
        IFileService fileService)
    {
        _envService = envService;
        _fileService = fileService;
    }

    /// <summary>
    /// Constructs a <see cref="ConfigurationParser"/> instance.
    /// </summary>
    public ConfigurationParser() : this(
        new EnvService(),
        new FileService())
    {
    }

    /// <summary>
    /// Converts a string array of command line arguments into a dictionary.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The parsed dictionary.</returns>
    public IDictionary<string, string> ParseConfiguration(string[] args)
    {
        var configMetadata = ConfigurationHelper.GetConfigKeyMetadata();
        var envResult = ParseEnvironment(configMetadata);
        var argsResult = args.Parse();
        var configFileResult = ParseConfigFile(envResult, argsResult);
        return DetermineFinalConfigDictionary(envResult, argsResult, configFileResult, configMetadata);
    }

    private IDictionary<string, string> ParseEnvironment(
        IList<ConfigMetadataModel> configMetadata)
    {
        var result = new Dictionary<string, string>();
        var envVars = _envService.GetEnvironmentVariables();
        if (envVars == null)
        {
            return new Dictionary<string, string>();
        }

        foreach (var constant in configMetadata)
        {
            if (string.IsNullOrWhiteSpace(constant.Key))
            {
                continue;
            }

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
        var key = ConfigKeys.ConfigJsonLocationKey;
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
        if (!_fileService.FileExists(configJsonPath))
        {
            throw new FileNotFoundException($"File '{configJsonPath}' not found.");
        }

        ConsoleHelpers.WriteLineColor($"Reading configuration from '{configJsonPath}'.", ConsoleColor.Green,
            ConsoleColor.Black);
        var config = _fileService.ReadAllText(configJsonPath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
    }

    private IDictionary<string, string> DetermineFinalConfigDictionary(
        IDictionary<string, string> envResult,
        IDictionary<string, string> argsResult,
        IDictionary<string, string> configFileResult,
        IList<ConfigMetadataModel> configMetadata)
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
                value = "True";
            }

            result.Add(constant.Key, value);
        }

        EnsureDefaultValuesAreAdded(result);
        return result.ToDictionary(
            r => configMetadata.Single(m => m.Key == r.Key).Path,
            r => r.Value);
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
        var windowsProfilePath = _envService.GetEnvironmentVariable("USERPROFILE");
        var unixProfilePath = _envService.GetEnvironmentVariable("HOME");
        const string stubFolderName = ".httplaceholder";
        if (_envService.IsOs(OSPlatform.Windows) && _fileService.DirectoryExists(windowsProfilePath))
        {
            fileStorageLocation = $"{windowsProfilePath}\\{stubFolderName}";
        }
        else if (
            (_envService.IsOs(OSPlatform.Linux) ||
             _envService.IsOs(OSPlatform.OSX)) && _fileService.DirectoryExists(unixProfilePath))
        {
            fileStorageLocation = $"{unixProfilePath}/{stubFolderName}";
        }

        if (!string.IsNullOrWhiteSpace(fileStorageLocation))
        {
            argsDictionary.EnsureEntryExists(ConfigKeys.FileStorageLocationKey, fileStorageLocation);
        }
    }
}
