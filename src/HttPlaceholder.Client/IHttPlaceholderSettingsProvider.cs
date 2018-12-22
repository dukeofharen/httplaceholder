using HttPlaceholder.Client.Models;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// Describes an interface that is used for retrieving the HttPlaceholder REST API settings.
    /// </summary>
    public interface IHttPlaceholderSettingsProvider
    {
        /// <summary>
        /// Gets the HttPlaceholder REST API settings.
        /// </summary>
        /// <returns>The <see cref="HttPlaceholderSettingsModel"/>.</returns>
        HttPlaceholderSettingsModel GetSettings();
    }
}
