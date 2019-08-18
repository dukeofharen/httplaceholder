using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest
{
    public class CreateStubForRequestCommand : IRequest
    {
        public string CorrelationId { get; set; }
    }
}
