using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.Export.Queries.ExportRequest;

/// <summary>
///     A query handler for exporting a request.
/// </summary>
public class ExportRequestQueryHandler : IRequestHandler<ExportRequestQuery, string>
{
    /// <inheritdoc />
    public Task<string> Handle(ExportRequestQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult("TODO"); // TODO
    }
}
