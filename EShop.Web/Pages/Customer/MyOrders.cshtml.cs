using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EShop.Web.Pages.Customer
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly IOrderService _orderService;

        public MyOrdersModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<Order> MyOrders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int customerId))
            {
                MyOrders = await _orderService.GetCustomerOrdersAsync(customerId);
            }
        }
    }
}