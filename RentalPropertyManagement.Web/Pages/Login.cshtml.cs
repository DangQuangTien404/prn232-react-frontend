using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RentalPropertyManagement.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public LoginDto LoginRequest { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
            // Kiểm tra xem có thông báo thành công từ trang Register không
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ErrorMessage = null; // Xóa thông báo lỗi nếu có thông báo thành công
            }
            else
            {
                ErrorMessage = "";
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userDto = await _userService.LoginAsync(LoginRequest);

            if (userDto == null)
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng. Vui lòng thử lại.";
                return Page();
            }

            // --- 1. Tạo Claims Identity ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                // Add explicit UserId claim to match Chat usage if needed, or stick to NameIdentifier
                new Claim("UserId", userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.FullName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Giữ đăng nhập
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Hết hạn sau 1 giờ
            };

            // --- 2. Thực hiện đăng nhập (Sign In) ---
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // --- 3. Chuyển hướng theo Role ---
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return userDto.Role switch
            {
                "Landlord" => RedirectToPage("/Landlord/Dashboard"),
                "Tenant" => RedirectToPage("/Tenant/Dashboard"),
                "ServiceProvider" => RedirectToPage("/Provider/Dashboard"),
                _ => RedirectToPage("/Index"),
            };
        }
    }
}