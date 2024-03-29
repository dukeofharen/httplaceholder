namespace HttPlaceholder.Persistence.Db.Entities;

/// <summary>
///     Represents a response in the database.
/// </summary>
public class DbResponseModel
{
    /// <summary>
    ///     Gets or sets the ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Gets or sets the distribution key.
    /// </summary>
    public string DistributionKey { get; set; }

    /// <summary>
    ///     Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the HTTP headers as JSON.
    /// </summary>
    public string Headers { get; set; }

    /// <summary>
    ///     Gets or sets the response body base64 encoded.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    ///     Gets or sets whether the response body is binary.
    /// </summary>
    public bool BodyIsBinary { get; set; }
}
