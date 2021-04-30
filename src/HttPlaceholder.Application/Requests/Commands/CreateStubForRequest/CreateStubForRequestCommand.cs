using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest
{
    public class CreateStubForRequestCommand : IRequest<FullStubModel>
    {
        public CreateStubForRequestCommand(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CorrelationId { get; }
    }
}
