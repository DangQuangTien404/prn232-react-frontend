using EShop.BLL.DTOs;
using EShop.DAL.Entities;
using EShop.DAL.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int customerId, List<CartItem> cartItems, PaymentMethod paymentMethod);
        Task<List<OrderDetail>> GetSellerOrdersAsync(int sellerId);
        Task UpdateStatusAsync(int orderId, OrderStatus status);
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
        Task<DashboardStatsDto> GetAdminStatsAsync();
    }
}