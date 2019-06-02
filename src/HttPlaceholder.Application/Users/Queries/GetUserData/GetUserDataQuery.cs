using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries.GetUserData
{
    public class GetUserDataQuery : IRequest<UserModel>
    {
        public string Username { get; set; }
    }
}
