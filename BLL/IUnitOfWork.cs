using DAL.Entities;
using BLL.Repositories;

namespace BLL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<OrderDetail> OrderDetailRepository { get; }
        IRepository<Review> ReviewRepository { get; }

        void Save(); 
    }
}