using System;
using System.Threading.Tasks; // Cần thêm using này
using RentalPropertyManagement.DAL.Entities; // Cần Entities để định nghĩa Repository

namespace RentalPropertyManagement.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Thêm các Repository cụ thể
        IRepository<User> Users { get; }
        IRepository<Property> Properties { get; }
        IRepository<Contract> Contracts { get; }
        IRepository<MaintenanceRequest> MaintenanceRequests { get; }

        // Cập nhật Save() thành CompleteAsync()
        Task<int> CompleteAsync();

        // Giữ lại Complete() sync (thay thế Save() cũ)
        int Complete();
    }
}