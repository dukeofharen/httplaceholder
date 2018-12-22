namespace HttPlaceholder.Client.Models
{
    /// <summary>
    /// A model that is used for storing HttPlaceholder REST API settings.
    /// </summary>
    public class HttPlaceholderSettingsModel
    {
        /// <summary>
        /// Gets or sets the root URL.
        /// </summary>
        public string RootUrl { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
