namespace HttPlaceholder.Client.Dto.Requests;

/// <summary>
///     A model for storing the execution result of a specific response writer.
/// </summary>
public class StubResponseWriterResultDto
{
    /// <summary>
    ///     Gets or sets the name of the response writer.
    /// </summary>
    public string ResponseWriterName { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="StubResponseWriterResultDto" /> is executed.
    /// </summary>
    public bool Executed { get; set; }

    /// <summary>
    ///     Gets or sets the log string of the executed response writer.
    /// </summary>
    public string Log { get; set; }
}
