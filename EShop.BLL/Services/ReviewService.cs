using EShop.DAL.Entities;
using EShop.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;

        public ReviewService(IReviewRepository reviewRepository, IOrderRepository orderRepository)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
        }

        // Khách viết đánh giá
        public async Task AddReviewAsync(int productId, int customerId, int rating, string comment)
        {
            // Kiểm tra khách hàng đã mua sản phẩm này chưa
            var orders = await _orderRepository.GetOrdersByCustomerAsync(customerId);
            var hasPurchased = orders.Any(o =>
                o.Status == DAL.Enums.OrderStatus.Completed &&
                o.OrderDetails.Any(od => od.ProductId == productId));

            if (!hasPurchased)
            {
                throw new InvalidOperationException("Bạn chưa mua sản phẩm này hoặc đơn hàng chưa hoàn tất nên không thể đánh giá.");
            }

            var review = new Review
            {
                ProductId = productId,
                CustomerId = customerId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsVisible = true
            };
            await _reviewRepository.AddReviewAsync(review);
        }

        // Lấy đánh giá sản phẩm
        public async Task<List<Review>> GetProductReviewsAsync(int productId)
        {
            return await _reviewRepository.GetReviewsByProductIdAsync(productId);
        }

        // Admin lấy tất cả
        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetAllReviewsAsync();
        }

        // Admin xóa
        public async Task DeleteReviewAsync(int id)
        {
            await _reviewRepository.DeleteReviewAsync(id);
        }

        // Seller lấy đánh giá của mình
        public async Task<List<Review>> GetReviewsBySellerAsync(int sellerId)
        {
            return await _reviewRepository.GetReviewsBySellerAsync(sellerId);
        }

        // Seller trả lời đánh giá
        public async Task ReplyReviewAsync(int reviewId, string reply)
        {
            // Bước 1: Tìm review đó
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            // Bước 2: Cập nhật nội dung trả lời
            if (review != null)
            {
                review.SellerReply = reply;
                review.ReplyDate = DateTime.Now;

                // Bước 3: Lưu xuống DB
                await _reviewRepository.UpdateReviewAsync(review);
            }
        }
    }
}