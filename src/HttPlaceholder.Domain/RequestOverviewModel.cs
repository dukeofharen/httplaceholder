using System;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing a stripped down version of a request.
/// </summary>
public class RequestOverviewModel
{
    /// <summary>
    ///     Gets or sets the correlation identifier.
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    ///     Gets or sets the method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    ///     Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets the executing stub identifier.
    /// </summary>
    public string ExecutingStubId { get; set; }

    /// <summary>
    ///     Gets or sets the tenant name of the stub.
    /// </summary>
    public string StubTenant { get; set; }

    /// <summary>
    ///     Gets or sets the request begin time.
    /// </summary>
    public DateTime RequestBeginTime { get; set; }

    /// <summary>
    ///     Gets or sets the request end time.
    /// </summary>
    public DateTime RequestEndTime { get; set; }

    /// <summary>
    ///     Gets or sets whether a response is saved for this request.
    /// </summary>
    public bool HasResponse { get; set; }
}
