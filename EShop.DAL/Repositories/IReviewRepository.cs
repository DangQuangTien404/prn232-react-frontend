using EShop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public interface IReviewRepository
    {

        Task AddReviewAsync(Review review);

        Task<List<Review>> GetReviewsByProductIdAsync(int productId);

        Task<List<Review>> GetAllReviewsAsync();

        Task DeleteReviewAsync(int id);

        Task<List<Review>> GetReviewsBySellerAsync(int sellerId);

        Task<Review> GetByIdAsync(int id);

        Task UpdateReviewAsync(Review review);
    }
}