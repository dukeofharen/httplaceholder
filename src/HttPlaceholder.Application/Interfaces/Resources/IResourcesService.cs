namespace HttPlaceholder.Application.Interfaces.Resources;

/// <summary>
/// Describes a class that is used to read resources from the file system.
/// </summary>
public interface IResourcesService
{
    /// <summary>
    /// Reads a resource as string based. The relative path is the path from the executable of HttPlaceholder.
    /// </summary>
    /// <param name="relativePath">The path from the executable of HttPlaceholder.</param>
    /// <returns>The resource as string.</returns>
    string ReadAsString(string relativePath);
}
