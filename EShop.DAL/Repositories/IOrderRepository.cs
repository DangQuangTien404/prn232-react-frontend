using EShop.DAL.Entities;
using EShop.DAL.Enums;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(Order order);
        Task<List<OrderDetail>> GetOrderDetailsBySellerAsync(int sellerId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<List<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<(decimal Revenue, int OrderCount, int ProductCount)> GetSalesStatisticsAsync();
    }
}