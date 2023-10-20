using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Export.Implementations;

internal class RequestToCurlCommandService : IRequestToCurlCommandService, ISingletonService
{
    public string Convert(RequestResultModel request) => throw new System.NotImplementedException();
}
