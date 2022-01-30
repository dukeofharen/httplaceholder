using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Controllers;

/// <summary>
/// Default base api controller
/// </summary>
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ApiController]
public abstract class BaseApiController : Controller
{
    private IMapper _mapper;
    private IMediator _mediator;

    /// <summary>
    /// Gets the AutoMapper instance.
    /// </summary>
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();

    /// <summary>
    /// Gets the Mediator instance.
    /// </summary>
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
