using EShop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task<List<Product>> GetProductsBySellerAsync(int sellerId);

        Task UpdateAsync(Product product);

        Task DeleteAsync(Product product);
    }
}