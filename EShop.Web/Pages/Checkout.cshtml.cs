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
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0) return RedirectToPage("/Index");

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int customerId)) return RedirectToPage("/Login");

            try
            {
                int orderId = await _orderService.CreateOrderAsync(customerId, cart, SelectedPaymentMethod);

                string notificationMsg = $"Đơn hàng #{orderId} vừa được tạo bởi {User.Identity.Name}!";
                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", notificationMsg);

                HttpContext.Session.Remove("Cart");

                return RedirectToPage("/OrderSuccess", new { id = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đặt hàng thất bại: " + ex.Message);

                CartItems = cart;
                return Page();
            }
        }
    }
}