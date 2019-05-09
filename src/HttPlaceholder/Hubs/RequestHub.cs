using System.Threading.Tasks;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs
{
    /// <summary>
    /// The request SignalR hub.
    /// </summary>
    public class RequestHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task RequestsUpdated(RequestResultModel request)
        {
            await Clients.All.SendAsync("RequestReceived", request);
        }
    }
}
