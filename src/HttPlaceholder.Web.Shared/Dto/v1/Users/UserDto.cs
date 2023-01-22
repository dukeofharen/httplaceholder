using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Dto.v1.Users;

/// <summary>
///     A model for storing information about a user.
/// </summary>
public class UserDto : IMapFrom<UserModel>
{
    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public string Username { get; set; }
}
