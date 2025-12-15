using RentalPropertyManagement.BLL.DTOs;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> LoginAsync(LoginDto request);

        Task<bool> RegisterAsync(RegisterDto request);
    }
}