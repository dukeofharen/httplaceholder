using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries.GetUserData
{
    public class GetUserDataQuery : IRequest<UserModel>
    {
        public GetUserDataQuery(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }
}
