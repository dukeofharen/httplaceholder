using System.Threading;
using System.Threading.Tasks;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that reads the HTTP input request input, runs the stub engine and writes the stub response to the HTTP response.
/// </summary>
public interface IStubHandler
{
    /// <summary>
    ///     Reads the HTTP input request input, runs the stub engine and writes the stub response to the HTTP response.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task HandleStubRequestAsync(CancellationToken cancellationToken);
}
