namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing the execution result of a specific response writer.
/// </summary>
public class StubResponseWriterResultModel
{
    /// <summary>
    ///     Gets or sets the name of the response writer.
    /// </summary>
    public string ResponseWriterName { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="StubResponseWriterResultModel" /> is executed.
    /// </summary>
    public bool Executed { get; set; }

    /// <summary>
    ///     Gets or sets the log string of the executed response writer.
    /// </summary>
    public string Log { get; set; }

    /// <summary>
    ///     Returns a "not executed" response.
    /// </summary>
    /// <param name="responseWriterName">The response writer name.</param>
    /// <returns>The <see cref="StubResponseWriterResultModel" />.</returns>
    public static StubResponseWriterResultModel IsNotExecuted(string responseWriterName) =>
        new() { Executed = false, ResponseWriterName = responseWriterName };

    /// <summary>
    ///     Returns a "not executed" response.
    /// </summary>
    /// <param name="responseWriterName">The response writer name.</param>
    /// <param name="log">The log line.</param>
    /// <returns>The <see cref="StubResponseWriterResultModel" />.</returns>
    public static StubResponseWriterResultModel IsNotExecuted(string responseWriterName, string log) =>
        new() { Executed = false, ResponseWriterName = responseWriterName, Log = log };

    /// <summary>
    ///     Returns an "executed" response.
    /// </summary>
    /// <param name="responseWriterName">The response writer name.</param>
    /// <returns>The <see cref="StubResponseWriterResultModel" />.</returns>
    public static StubResponseWriterResultModel IsExecuted(string responseWriterName) =>
        new() { Executed = true, ResponseWriterName = responseWriterName };

    /// <summary>
    ///     Returns an "executed" response.
    /// </summary>
    /// <param name="responseWriterName">The response writer name.</param>
    /// <param name="log">The log line.</param>
    /// <returns>The <see cref="StubResponseWriterResultModel" />.</returns>
    public static StubResponseWriterResultModel IsExecuted(string responseWriterName, string log) =>
        new() { Executed = true, ResponseWriterName = responseWriterName, Log = log };
}
