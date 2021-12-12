using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetAllStubs;

public class GetAllStubsQuery : IRequest<IEnumerable<FullStubModel>>
{
}