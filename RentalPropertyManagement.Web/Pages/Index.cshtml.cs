using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; // Cần dùng để bảo vệ trang

namespace RentalPropertyManagement.Web.Pages
{
    // Yêu cầu xác thực để truy cập trang Index (Dashboard)
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string UserRole { get; set; }
        public string UserFullName { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Nếu chưa, chuyển hướng đến trang Login
                return RedirectToPage("/Login");
            }

            // Lấy thông tin Role từ Claims (đã lưu khi Login)
            UserRole = User.FindFirstValue(ClaimTypes.Role);
            UserFullName = User.FindFirstValue(ClaimTypes.Name);

            // Nếu Role không xác định, chuyển hướng đến trang từ chối truy cập
            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToPage("/AccessDenied");
            }

            // Tiếp tục hiển thị trang với các thông tin đã lấy
            return Page();
        }
    }
}