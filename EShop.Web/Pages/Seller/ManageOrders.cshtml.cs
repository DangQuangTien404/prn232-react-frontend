using EShop.BLL.Services;
using EShop.DAL.Entities;
using EShop.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class ManageOrdersModel : PageModel
    {
        private readonly IOrderService _orderService;

        public ManageOrdersModel(IOrderService orderService)
        {
            _orderService = orderService;
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

        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, int statusId)
        {
            var newStatus = (OrderStatus)statusId;
            await _orderService.UpdateStatusAsync(orderId, newStatus);
            return RedirectToPage();
        }
    }
}