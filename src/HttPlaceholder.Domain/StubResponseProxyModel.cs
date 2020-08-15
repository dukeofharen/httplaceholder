namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing proxy settings
    /// </summary>
    public class StubResponseProxyModel
    {
        /// <summary>
        /// Gets or sets the URL where the request should be sent to. The request will be sent to exactly this URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the root URL where the request should be sent to. The request will be sent to this URL with the path + query string of the request to HttPlaceholder appended to it.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
