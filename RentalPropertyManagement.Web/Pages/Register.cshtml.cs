using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public RegisterDto RegisterRequest { get; set; }

        public string ErrorMessage { get; set; }

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
            // Khởi tạo Role mặc định là Tenant
            RegisterRequest = new RegisterDto { Role = UserRole.Tenant };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Bỏ qua Validation cho Role vì nó được set ngầm và cố định cho trang này
            ModelState.Remove("RegisterRequest.Role");

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng kiểm tra lại thông tin đăng ký.";
                return Page();
            }

            // Luôn đảm bảo Role là Tenant khi đăng ký từ trang công cộng này
            RegisterRequest.Role = UserRole.Tenant;

            var result = await _userService.RegisterAsync(RegisterRequest);

            if (result)
            {
                // Đăng ký thành công, lưu thông báo vào TempData và chuyển hướng về Login
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng sử dụng Email và Mật khẩu vừa tạo để đăng nhập.";
                return RedirectToPage("/Login");
            }
            else
            {
                // Lỗi thường gặp: Email đã tồn tại (logic được xử lý trong UserService)
                ErrorMessage = "Email đã tồn tại trong hệ thống. Vui lòng sử dụng email khác.";
                return Page();
            }
        }
    }
}