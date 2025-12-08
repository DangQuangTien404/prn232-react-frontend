using EShop.BLL.DTOs;
using EShop.DAL.Entities;
using EShop.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
        public async Task<bool> RegisterAsync(RegisterDto request)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return false;
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password,
                FullName = request.FullName,
                Role = request.Role
            };

            await _userRepository.AddAsync(user);
            return true;
        }

        public async Task<User> LoginAsync(LoginDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return null;
            }

            if (user.PasswordHash != request.Password)
            {
                return null;
            }

            return user;
        }
    }
}