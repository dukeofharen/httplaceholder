using AutoMapper;
using HttPlaceholder.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// Default base api controller
    /// </summary>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [ApiAuthorization]
    public class BaseApiController : Controller
    {
        private IMapper _mapper;
        private IMediator _mediator;

        protected IMapper Mapper => _mapper ?? (_mapper = HttpContext.RequestServices.GetRequiredService<IMapper>());

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetRequiredService<IMediator>());
    }
}
