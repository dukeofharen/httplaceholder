using System;

namespace HttPlaceholder.Domain.Entities;

/// <summary>
/// Represents a request in the database.
/// </summary>
public class DbRequestModel
{
    /// <summary>
    /// Gets or sets the request ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the executing stub ID.
    /// </summary>
    public string ExecutingStubId { get; set; }

    /// <summary>
    /// Gets or sets the begin time of the request.
    /// </summary>
    public DateTime RequestBeginTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the request.
    /// </summary>
    public DateTime RequestEndTime { get; set; }

    /// <summary>
    /// Gets or sets the JSON representation of the request.
    /// </summary>
    public string Json { get; set; }
}