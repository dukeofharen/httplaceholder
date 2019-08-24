using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest
{
    public class CreateStubForRequestCommand : IRequest<FullStubModel>
    {
        public string CorrelationId { get; set; }
    }
}
