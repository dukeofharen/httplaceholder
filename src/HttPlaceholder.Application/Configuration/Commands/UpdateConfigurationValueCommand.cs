using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HttPlaceholder.Application.Configuration.Commands;

/// <summary>
///     A command for updating a configuration value runtime.
/// </summary>
public class UpdateConfigurationValueCommand : IRequest<Unit>
{
    /// <summary>
    ///     Constructs a <see cref="UpdateConfigurationValueCommand" /> object.
    /// </summary>
    public UpdateConfigurationValueCommand(string configurationKey, string newValue)
    {
        ConfigurationKey = configurationKey;
        NewValue = newValue;
    }

    /// <summary>
    ///     The configuration key.
    /// </summary>
    public string ConfigurationKey { get; }

    /// <summary>
    ///     The new configuration value.
    /// </summary>
    public string NewValue { get; }
}

/// <summary>
///     A handler for updating a configuration value runtime.
/// </summary>
public class UpdateConfigurationValueCommandHandler : IRequestHandler<UpdateConfigurationValueCommand, Unit>
{
    private static readonly string[] _expectedBoolValues = ["true", "false"];
    private readonly IConfigurationRoot _configuration;

    /// <summary>
    ///     Constructs a <see cref="UpdateConfigurationValueCommandHandler" /> object.
    /// </summary>
    /// <param name="configuration"></param>
    public UpdateConfigurationValueCommandHandler(
        IConfiguration configuration)
    {
        _configuration = (IConfigurationRoot)configuration;
    }

    /// <inheritdoc />
    public Task<Unit> Handle(UpdateConfigurationValueCommand request, CancellationToken cancellationToken)
    {
        var metadata = ConfigKeys.GetConfigMetadata().FirstOrDefault(m =>
            string.Equals(m.Key, request.ConfigurationKey, StringComparison.OrdinalIgnoreCase));
        if (metadata == null)
        {
            throw new NotFoundException($"Configuration value with key '{request.ConfigurationKey}'.");
        }

        if (metadata.CanBeMutated != true)
        {
            throw new InvalidOperationException(
                $"Configuration value with key '{request.ConfigurationKey}' can not be mutated at this moment.");
        }

        if (metadata.IsBoolValue == true &&
            !_expectedBoolValues.Any(v => string.Equals(v, request.NewValue, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                $"Configuration value with key '{request.ConfigurationKey}' is of type boolean, but no boolean value was passed.");
        }

        var type = typeof(MemoryConfigurationProvider);
        var provider = _configuration.Providers.FirstOrDefault(p => p.GetType() == type);
        if (provider == null)
        {
            throw new InvalidOperationException(
                $"Configuration provider with type '{type.Name}' unexpectedly not found.");
        }

        provider.Set(metadata.Path, request.NewValue);
        _configuration.Reload();
        return Unit.Task;
    }
}
