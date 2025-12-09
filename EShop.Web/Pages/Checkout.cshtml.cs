using EShop.BLL.DTOs;
using EShop.BLL.Services;
using EShop.DAL.Enums;
using EShop.Web.Helpers;
using EShop.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace EShop.Web.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<ECommerceHub> _hubContext;

        public CheckoutModel(IOrderService orderService, IHubContext<ECommerceHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal GrandTotal => CartItems.Sum(x => x.Total);

        [BindProperty]
        public PaymentMethod SelectedPaymentMethod { get; set; }

        public IActionResult OnGet()
        {
            CartItems = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (CartItems.Count == 0)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Lấy lại giỏ hàng
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0) return RedirectToPage("/Index");

            // 2. Lấy ID khách hàng
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int customerId)) return RedirectToPage("/Login");

            try
            {
                // 3. Gọi Service tạo đơn hàng (Có Transaction bên trong Service)
                int orderId = await _orderService.CreateOrderAsync(customerId, cart, SelectedPaymentMethod);

                // 4. Gửi thông báo SignalR (Real-time)
                // CẬP NHẬT: Tên method phải khớp với 'connection.on("ReceiveOrderNotification"...)' trong file site.js
                string notificationMsg = $"Đơn hàng #{orderId} vừa được tạo bởi {User.Identity.Name}!";
                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", notificationMsg);

                // 5. Xóa giỏ hàng sau khi đặt thành công
                HttpContext.Session.Remove("Cart");

                // 6. Chuyển hướng
                return RedirectToPage("/OrderSuccess", new { id = orderId });
            }
            catch (Exception ex)
            {
                // XỬ LÝ LỖI: Nếu tạo đơn thất bại (ví dụ: Hết hàng), code sẽ nhảy vào đây
                ModelState.AddModelError("", "Đặt hàng thất bại: " + ex.Message);

                // Quan trọng: Phải gán lại CartItems để hiển thị lại trang mà không bị lỗi Null
                CartItems = cart;
                return Page();
            }
        }
    }
}