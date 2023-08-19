using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Extensibility.Notifications;

/// <summary>
///     This notification is sent just before a stub response is sent back to the client. You have the opportunity to
///     modify the response here.
///     This notification is only sent if a valid stub was found.
/// </summary>
public class BeforeStubResponseReturnedNotification : INotification
{
    /// <summary>
    ///     Gets or sets the response that will be returned to the client.
    /// </summary>
    public ResponseModel Response { get; set; }
}
