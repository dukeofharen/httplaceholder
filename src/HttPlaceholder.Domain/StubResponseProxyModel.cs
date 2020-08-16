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

        /// <summary>
        /// Gets or sets whether the query string of the request to HttPlaceholder should be appended to the string that will be send to the proxy URL. This can only be used in combination with the property <see cref="Url"/>.
        /// </summary>
        public bool? AppendQueryString { get; set; }

        /// <summary>
        /// Gets or sets whether the root URL of the response of the target web service should be replaced with the root URL of HttPlaceholder.
        /// </summary>
        public bool? ReplaceRootUrl { get; set; }
    }
}
