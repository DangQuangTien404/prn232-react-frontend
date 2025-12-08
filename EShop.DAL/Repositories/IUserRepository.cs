using EShop.DAL.Entities;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task DeleteUserAsync(int id);
    }
}