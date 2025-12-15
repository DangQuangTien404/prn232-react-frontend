using System.ComponentModel.DataAnnotations;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        // Mặc định cho Tenant khi đăng ký qua trang web
        public UserRole Role { get; set; } = UserRole.Tenant;

        public string PhoneNumber { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}