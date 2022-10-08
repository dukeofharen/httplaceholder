using System.Threading;
using System.Threading.Tasks;

namespace HttPlaceholder.Application.Interfaces.Persistence;

/// <summary>
/// Describes a class that is used to retrieve the root paths of the locations the stub YAML files are located in.
/// </summary>
public interface IStubRootPathResolver
{
    /// <summary>
    /// Returns a list of root paths the stub YAML files are located in. If no YAML files are provided, the root path of HttPlaceholder is returned instead.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stub root paths.</returns>
    Task<string[]> GetStubRootPathsAsync(CancellationToken cancellationToken);
}
