using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries.GetUserData
{
    public class GetUserDataQuery : IRequest<UserModel>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Username { get; set; }
    }
}
