using EShop.BLL.Services;
using EShop.DAL.Entities;
using EShop.DAL.Enums;
using EShop.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR; // Import SignalR
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class ManageOrdersModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<ECommerceHub> _hubContext; // Khai báo Hub

        public ManageOrdersModel(IOrderService orderService, IHubContext<ECommerceHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext; // Inject Hub
        }

        public List<OrderDetail> SellerOrderDetails { get; set; } = new List<OrderDetail>();

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int sellerId))
            {
                SellerOrderDetails = await _orderService.GetSellerOrdersAsync(sellerId);
            }
        }

        public async Task<IActionResult> OnPostUpdateStatus(int orderId, int statusId)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), statusId))
            {
                return RedirectToPage();
            }

            var status = (OrderStatus)statusId;

            // 1. Cập nhật trong Database (đã bao gồm logic hoàn kho nếu Hủy)
            await _orderService.UpdateStatusAsync(orderId, status);

            // 2. Gửi tín hiệu SignalR cho Client
            // Gửi: Mã đơn hàng + Tên trạng thái mới
            await _hubContext.Clients.All.SendAsync("ReceiveOrderStatusUpdate", orderId, status.ToString());

            TempData["SuccessMessage"] = $"Đơn hàng #{orderId} đã cập nhật thành công!";
            return RedirectToPage();
        }
    }
}