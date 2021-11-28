using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands
{
    public class CreateCurlStubCommandHandler : IRequestHandler<CreateCurlStubCommand, IEnumerable<FullStubModel>>
    {
        public Task<IEnumerable<FullStubModel>> Handle(CreateCurlStubCommand request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
