using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// Describes a class that is used to make REST calls to the HttPlaceholder REST API.
    /// </summary>
    public interface IHttPlaceholderClient
    {
        /// <summary>
        /// Checks whether the given credentials belong to a valid user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="UserModel"/>.</returns>
        /// <remarks>
        /// If no authentication is configured on HttPlaceholder, this call will always return a successful result.
        /// </remarks>
        Task<UserModel> GetUserAsync(string username, string password);
    }
}
