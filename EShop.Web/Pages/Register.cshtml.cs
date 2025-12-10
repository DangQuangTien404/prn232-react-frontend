using EShop.BLL.DTOs;
using EShop.BLL.Services;
using EShop.DAL.Enums; // Để dùng Enum UserRole
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterDto RegisterInput { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userService.RegisterAsync(RegisterInput);
            if (!result)
            {
                Message = "Tài khoản đã tồn tại.";
                return Page();
            }

            return RedirectToPage("/Login");
        }
    }
}