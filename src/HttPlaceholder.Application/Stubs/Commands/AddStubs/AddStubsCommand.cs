using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStubs
{
    public class AddStubsCommand : IRequest<IEnumerable<FullStubModel>>
    {
        public AddStubsCommand(IEnumerable<StubModel> stubs)
        {
            Stubs = stubs;
        }

        public IEnumerable<StubModel> Stubs { get; }
    }
}
