using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RentalPropertyManagement.Web.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to a group named by their UserId
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }
    }
}
