using BLL.Repositories;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace BLL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        private IRepository<User> _userRepository;
        private IRepository<Product> _productRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderDetail> _orderDetailRepository;
        private IRepository<Review> _reviewRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<User> UserRepository => _userRepository ??= new GenericRepository<User>(_context);
        public IRepository<Product> ProductRepository => _productRepository ??= new GenericRepository<Product>(_context);
        public IRepository<Order> OrderRepository => _orderRepository ??= new GenericRepository<Order>(_context);
        public IRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository ??= new GenericRepository<OrderDetail>(_context);
        public IRepository<Review> ReviewRepository => _reviewRepository ??= new GenericRepository<Review>(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _transaction?.Rollback();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}