using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages
{
    // Trang Logout không cần giao diện (View), chỉ cần PageModel xử lý logic
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Thực hiện đăng xuất bằng cách xóa cookie xác thực
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng người dùng về trang chủ hoặc trang đăng nhập
            return RedirectToPage("/Login");
        }

        // Cần phương thức OnGet để người dùng có thể truy cập /Logout trực tiếp
        public async Task<IActionResult> OnGetAsync()
        {
            return await OnPostAsync();
        }
    }
}