using EShop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public interface IReviewService
    {
        Task AddReviewAsync(int productId, int customerId, int rating, string comment);

        Task<List<Review>> GetProductReviewsAsync(int productId);

        Task<List<Review>> GetAllReviewsAsync();

        Task DeleteReviewAsync(int id);
        Task<List<Review>> GetReviewsBySellerAsync(int sellerId);
        Task ReplyReviewAsync(int reviewId, string reply);
    }
}