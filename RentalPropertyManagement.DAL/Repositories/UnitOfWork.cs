using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Entities; // Cần Entities để khởi tạo Repository
using System.Threading.Tasks;

namespace RentalPropertyManagement.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RentalDbContext _context;

        // Khai báo các Repository
        public IRepository<User> Users { get; private set; }
        public IRepository<Property> Properties { get; private set; }
        public IRepository<Contract> Contracts { get; private set; }
        public IRepository<MaintenanceRequest> MaintenanceRequests { get; private set; }

        public UnitOfWork(RentalDbContext context)
        {
            _context = context;

            // Khởi tạo các Repository
            Users = new Repository<User>(_context);
            Properties = new Repository<Property>(_context);
            Contracts = new Repository<Contract>(_context);
            MaintenanceRequests = new Repository<MaintenanceRequest>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
      

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}