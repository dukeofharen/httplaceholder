using System;

namespace HttPlaceholder.Client.Exceptions
{
    /// <summary>
    /// An exception for signaling to the calling party if something went wrong when calling the HttPlacedholder REST API.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HttPlaceholderClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttPlaceholderClientException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public HttPlaceholderClientException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttPlaceholderClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttPlaceholderClientException(string message) : base(message)
        {
        }
    }
}
