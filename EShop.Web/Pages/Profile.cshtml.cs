using EShop.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EShop.Web.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;

        public ProfileModel(IUserService userService)
        {
            _userService = userService;
        }

        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            Username = user.Username;
            FullName = user.FullName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            user.FullName = FullName;
            await _userService.UpdateUserAsync(user);

            // Update user in memory is not enough if claims are used for display name, but usually FullName is fetched.
            // If claims store FullName, we might need to refresh sign-in, but here we just update DB.

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            Username = user.Username; // Restore username for display

            return Page();
        }
    }
}
