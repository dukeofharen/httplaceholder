using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries.GetUserData
{
    // ReSharper disable once UnusedType.Global
    public class GetUserDataQueryHandler : IRequestHandler<GetUserDataQuery, UserModel>
    {
        private readonly IUserContext _userContext;

        public GetUserDataQueryHandler(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public Task<UserModel> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
        {
            var nameClaim = _userContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (!string.IsNullOrWhiteSpace(nameClaim?.Value) && request.Username != nameClaim.Value)
            {
                throw new ForbiddenException();
            }

            return Task.FromResult(new UserModel
            {
                Username = request.Username
            });
        }
    }
}
