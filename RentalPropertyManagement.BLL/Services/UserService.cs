using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Interfaces;
using BCrypt.Net; // Cần package BCrypt.Net-Next

namespace RentalPropertyManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- Logic Đăng ký (Register) ---
        public async Task<bool> RegisterAsync(RegisterDto request)
        {
            // 1. Kiểm tra User đã tồn tại (Email là duy nhất)
            var existingUser = await _unitOfWork.Users.GetSingleAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return false; // User đã tồn tại
            }

            // 2. Hash mật khẩu trước khi lưu vào DB
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo Entity mới
            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber
            };

            // 4. Lưu vào Database
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // --- Logic Đăng nhập (Login) ---
        public async Task<UserDto> LoginAsync(LoginDto request)
        {
            var userEntity = await _unitOfWork.Users.GetSingleAsync(u => u.Email == request.Email);

            if (userEntity == null)
            {
                return null;
            }

            // **KIỂM TRA MẬT KHẨU BẰNG HASHED PASSWORD**
            if (!BCrypt.Net.BCrypt.Verify(request.Password, userEntity.PasswordHash))
            {
                return null;
            }
            // -----------------------------

            // Ánh xạ Entity sang DTO để trả về cho tầng Web
            return new UserDto
            {
                Id = userEntity.Id,
                FullName = $"{userEntity.FirstName} {userEntity.LastName}",
                Email = userEntity.Email,
                Role = userEntity.Role.ToString()
            };
        }
    }
}