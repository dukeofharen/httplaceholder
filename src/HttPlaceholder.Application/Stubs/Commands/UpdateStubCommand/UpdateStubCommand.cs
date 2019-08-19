using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand
{
    public class UpdateStubCommand : IRequest
    {
        public string StubId { get; set; }

        public StubModel Stub { get; set; }
    }
}
