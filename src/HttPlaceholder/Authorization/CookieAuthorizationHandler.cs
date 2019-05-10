using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HttPlaceholder.Authorization
{
    public class CookieAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            //context.Succeed();
            return Task.CompletedTask;
        }
    }
}
