using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStubsOverview
{
    public class GetStubsOverviewQuery : IRequest<IEnumerable<FullStubOverviewModel>>
    {
    }
}
