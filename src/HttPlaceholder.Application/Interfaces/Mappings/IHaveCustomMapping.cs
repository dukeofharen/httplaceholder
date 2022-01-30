using AutoMapper;

namespace HttPlaceholder.Application.Interfaces.Mappings;

/// <summary>
/// Describes a class that has a custom AutoMapper mapping.
/// </summary>
public interface IHaveCustomMapping
{
    /// <summary>
    /// Create an AutoMapper mapping.
    /// </summary>
    /// <param name="configuration">The AutoMapper profile.</param>
    void CreateMappings(Profile configuration);
}
