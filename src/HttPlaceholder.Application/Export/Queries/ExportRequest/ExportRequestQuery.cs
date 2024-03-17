using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Export.Queries.ExportRequest;

/// <summary>
///     A query for exporting a request.
/// </summary>
public class ExportRequestQuery : IRequest<string>
{
    /// <summary>
    ///     Constructs a <see cref="ExportRequestQuery" /> instance.
    /// </summary>
    public ExportRequestQuery(string correlationId, RequestExportType requestExportType)
    {
        CorrelationId = correlationId;
        RequestExportType = requestExportType;
    }

    /// <summary>
    ///     Gets the correlation ID.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    ///     Gets the request export type.
    /// </summary>
    public RequestExportType RequestExportType { get; }
}
