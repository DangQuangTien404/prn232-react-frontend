using BLL.Repositories;
using DAL;
using DAL.Entities;


namespace BLL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}