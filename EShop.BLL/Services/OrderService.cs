using EShop.BLL.DTOs;
using EShop.DAL.Entities;
using EShop.DAL.Enums;
using EShop.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository) // Thêm tham số
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }
        public async Task<DashboardStatsDto> GetAdminStatsAsync()
        {
            var stats = await _orderRepository.GetSalesStatisticsAsync();
            var users = await _userRepository.GetAllUsersAsync(); 

            return new DashboardStatsDto
            {
                TotalRevenue = stats.Revenue,
                TotalOrders = stats.OrderCount,
                TotalProductsSold = stats.ProductCount,
                TotalCustomers = users.Count(u => u.Role == DAL.Enums.UserRole.Customer)
            };
        }
        public async Task<List<OrderDetail>> GetSellerOrdersAsync(int sellerId)
        {
            return await _orderRepository.GetOrderDetailsBySellerAsync(sellerId);
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            return await _orderRepository.GetOrdersByCustomerAsync(customerId);
        }
        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }
        public async Task<int> CreateOrderAsync(int customerId, List<CartItem> cartItems, PaymentMethod paymentMethod)
        {
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                PaymentMethod = paymentMethod,
                IsPaid = false,
                TotalAmount = cartItems.Sum(x => x.Total),
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            return await _orderRepository.CreateOrderAsync(order);
        }
    }
}