using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class MimeService : IMimeService, ISingletonService
{
    /// <inheritdoc />
    public string GetMimeType(string input) => MimeTypes.GetMimeType(input);
}
