using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EShop.Web.Hubs
{
    public class ECommerceHub : Hub
    {
        // Sau này chúng ta sẽ viết hàm gửi thông báo đơn hàng mới tại đây.
        // Ví dụ: Gửi thông báo cho Seller khi có người đặt hàng.
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}