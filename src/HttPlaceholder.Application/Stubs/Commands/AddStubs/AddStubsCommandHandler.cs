using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStubs
{
    public class AddStubsCommandHandler : IRequestHandler<AddStubsCommand, IEnumerable<FullStubModel>>
    {
        public Task<IEnumerable<FullStubModel>> Handle(AddStubsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
