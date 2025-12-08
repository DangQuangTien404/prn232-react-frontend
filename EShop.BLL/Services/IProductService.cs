using EShop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);

        Task<List<Product>> GetProductsBySellerAsync(int sellerId);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
    }
}