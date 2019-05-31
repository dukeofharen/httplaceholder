namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing the execution result of a specific response writer.
    /// </summary>
    public class StubResponseWriterResultModel
    {
        /// <summary>
        /// Gets or sets the name of the response writer.
        /// </summary>
        public string ResponseWriterName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StubResponseWriterResultModel"/> is executed.
        /// </summary>
        public bool Executed { get; set; }
    }
}