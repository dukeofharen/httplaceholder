﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequestsOverview;

/// <summary>
///     A query handler for retrieving a list of overview requests.
/// </summary>
public class
    GetRequestsOverviewQueryHandler : IRequestHandler<GetRequestsOverviewQuery, IEnumerable<RequestOverviewModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetRequestsOverviewQueryHandler" /> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public GetRequestsOverviewQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> Handle(GetRequestsOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var pagingModel = !string.IsNullOrWhiteSpace(request.FromIdentifier) || request.ItemsPerPage.HasValue
            ? new PagingModel {FromIdentifier = request.FromIdentifier, ItemsPerPage = request.ItemsPerPage}
            : null;
        return await _stubContext.GetRequestResultsOverviewAsync(pagingModel, cancellationToken);
    }
}
