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
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository) // Thêm tham số
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
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
            // Nếu hủy đơn, trả hàng về kho
            if (status == OrderStatus.Cancelled)
            {
                var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
                if (order != null && order.Status != OrderStatus.Cancelled) // Chỉ hoàn kho nếu trạng thái trước đó chưa hủy
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += detail.Quantity;
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                }
            }

            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }

        public async Task<int> CreateOrderAsync(int customerId, List<CartItem> cartItems, PaymentMethod paymentMethod)
        {
            using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                // Validate Stock
                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                    {
                        // Tạm thời throw exception, hoặc xử lý trả về -1 để Controller redirect
                        // Nhưng yêu cầu "redirected to Cart with error message" được xử lý ở đây không tiện.
                        // Controller sẽ bắt exception này? Hay mình trả về ID lỗi?
                        throw new Exception($"Sản phẩm {product?.Name ?? "Không tìm thấy"} không đủ số lượng tồn kho.");
                    }
                }

                // Trừ kho
                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }

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

                var orderId = await _orderRepository.CreateOrderAsync(order);
                transaction.Complete();
                return orderId;
            }
        }
    }
}