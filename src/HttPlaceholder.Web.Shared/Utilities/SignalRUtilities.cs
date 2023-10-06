using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Web.Shared.Utilities;

/// <summary>
///     A simple utility class with several SignalR related methods.
/// </summary>
public static class SignalRUtilities
{
    /// <summary>
    ///     Determines the channel / client proxy for SignalR based on the group.
    /// </summary>
    /// <param name="hubContext">The SignalR hub context.</param>
    /// <param name="group">The SignalR group.</param>
    /// <returns>The <see cref="IClientProxy" />.</returns>
    public static IClientProxy GetChannel<THub>(this IHubContext<THub> hubContext, string group = null)
        where THub : Hub =>
        string.IsNullOrWhiteSpace(group)
            ? hubContext.Clients.All
            : hubContext.Clients.Group(group);
}
