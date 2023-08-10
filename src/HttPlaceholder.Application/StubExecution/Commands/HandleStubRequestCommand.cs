using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.StubExecution.Commands;

/// <summary>
///     A command that is used to execute the request against the registered stubs.
/// </summary>
public class HandleStubRequestCommand : IRequest<ResponseModel>
{
}
