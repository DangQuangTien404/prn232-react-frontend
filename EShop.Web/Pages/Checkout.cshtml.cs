using EShop.BLL.DTOs;
using EShop.BLL.Services;
using EShop.DAL.Enums;
using EShop.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    [Authorize] // Bắt buộc đăng nhập mới được thanh toán
    public class CheckoutModel : PageModel
    {
        private readonly IOrderService _orderService;

        public CheckoutModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<CartItem> CartItems { get; set; }
        public decimal GrandTotal => CartItems.Sum(x => x.Total);

        [BindProperty]
        public PaymentMethod SelectedPaymentMethod { get; set; }

        public IActionResult OnGet()
        {
            // Lấy giỏ hàng
            CartItems = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (CartItems.Count == 0)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Lấy lại giỏ hàng (vì HTTP là stateless)
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0) return RedirectToPage("/Index");

            // 2. Lấy ID khách hàng
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int customerId)) return RedirectToPage("/Login");

            // 3. Gọi Service tạo đơn hàng
            int orderId = await _orderService.CreateOrderAsync(customerId, cart, SelectedPaymentMethod);

            // 4. Xóa giỏ hàng sau khi đặt thành công
            HttpContext.Session.Remove("Cart");

            // 5. Chuyển hướng
            // Nếu chọn thanh toán Online (VNPay/Momo), sẽ chuyển sang trang thanh toán (làm sau).
            // Tạm thời mình chuyển về trang "Thành công" hết.
            return RedirectToPage("/OrderSuccess", new { id = orderId });
        }
    }
}