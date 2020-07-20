using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace HttPlaceholder.Dto.v1.Stubs
{
    /// <summary>
    /// A model for storing stub information for the basic authentication condition checker.
    /// </summary>
    public class StubBasicAuthenticationDto : IMapFrom<StubBasicAuthenticationModel>, IMapTo<StubBasicAuthenticationModel>
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [YamlMember(Alias = "username")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [YamlMember(Alias = "password")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Password { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString() => $"[Username = '{Username}', Password = '{Password}']";
    }
}
