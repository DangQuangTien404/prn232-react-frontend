using EShop.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EShopDbContext _context;

        public ReviewRepository(EShopDbContext context)
        {
            _context = context;
        }

        // 1. Thêm review
        public async Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        // 2. Lấy review theo sản phẩm (cho trang chi tiết)
        public async Task<List<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.Customer) // Lấy tên khách
                .Where(r => r.ProductId == productId && r.IsVisible)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // 3. Lấy tất cả review (cho Admin)
        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product) // Lấy tên sản phẩm để Admin biết review cái gì
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // 4. Xóa review (cho Admin)
        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        // 5. Lấy review của Seller (cho Seller quản lý)
        public async Task<List<Review>> GetReviewsBySellerAsync(int sellerId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Customer)
                .Where(r => r.Product.SellerId == sellerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // 6. Lấy review theo ID (để tìm trước khi update)
        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        // 7. Cập nhật review (Lưu câu trả lời)
        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}