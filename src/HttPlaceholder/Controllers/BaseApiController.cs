using System;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Common.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Controllers;

/// <summary>
///     Default base api controller
/// </summary>
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ApiController]
public abstract class BaseApiController : Controller
{
    private IMapper _mapper;
    private IMediator _mediator;

    /// <summary>
    ///     Gets the AutoMapper instance.
    /// </summary>
    private IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();

    /// <summary>
    ///     Gets the Mediator instance.
    /// </summary>
    private IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    ///     Sends a request to MediatR.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The MediatR response.</returns>
    protected Task<TResponse> Send<TResponse>(IRequest<TResponse> request) =>
        Mediator.Send(request, HttpContext.RequestAborted);

    /// <summary>
    ///     Maps an object to another object.
    /// </summary>
    /// <param name="source">The object to map.</param>
    /// <returns>The mapped object.</returns>
    protected TDestination Map<TDestination>(object source) => Mapper.Map<TDestination>(source);

    /// <summary>
    ///     Maps an object to another object.
    /// </summary>
    /// <param name="source">The object to map.</param>
    /// <param name="action">An extra mapping action on top of the base mapping.</param>
    /// <returns>The mapped object.</returns>
    protected TDestination MapAndSet<TDestination>(
        object source,
        Action<TDestination> action)
    {
        var destination = Mapper.Map<TDestination>(source);
        action(destination);
        return destination;
    }
}
