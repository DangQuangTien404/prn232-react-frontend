using EShop.DAL.Entities;
using EShop.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EShopDbContext _context;

        public OrderRepository(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<(decimal Revenue, int OrderCount, int ProductCount)> GetSalesStatisticsAsync()
        {
            var completedOrders = _context.Orders.Where(o => o.Status == OrderStatus.Completed);

            var revenue = await completedOrders.SumAsync(o => o.TotalAmount);

            var orderCount = await completedOrders.CountAsync();

            var productCount = await _context.OrderDetails
                .Where(od => od.Order.Status == OrderStatus.Completed)
                .SumAsync(od => od.Quantity);

            return (revenue, orderCount, productCount);
        }
        public async Task<List<OrderDetail>> GetOrderDetailsBySellerAsync(int sellerId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product) 
                .Include(od => od.Order)   
                .ThenInclude(o => o.Customer) 
                .Where(od => od.Product.SellerId == sellerId)
                .OrderByDescending(od => od.Order.OrderDate) 
                .ToListAsync();
        }
        public async Task<List<Order>> GetOrdersByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) // Lấy tên sản phẩm bên trong chi tiết
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate) // Đơn mới nhất lên đầu
                .ToListAsync();
        }
        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }
    }
}