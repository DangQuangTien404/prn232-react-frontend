using EShop.BLL.DTOs; // Thêm namespace
using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;

        public DashboardModel(IUserService userService, IReviewService reviewService, IOrderService orderService)
        {
            _userService = userService;
            _reviewService = reviewService;
            _orderService = orderService;
        }

        public List<User> Users { get; set; } = new List<User>();
        public List<Review> Reviews { get; set; } = new List<Review>();

        public DashboardStatsDto Stats { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userService.GetAllUsersAsync();
            Reviews = await _reviewService.GetAllReviewsAsync();

            Stats = await _orderService.GetAdminStatsAsync();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return RedirectToPage();
        }
    }
}