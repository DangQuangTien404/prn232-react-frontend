using EShop.BLL.DTOs;
using EShop.DAL.Entities;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto request);
        Task<User> LoginAsync(LoginDto request);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task DeleteUserAsync(int id);
    }
}