using EShop.DAL.Entities;
using EShop.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }
        public async Task<List<Product>> GetProductsBySellerAsync(int sellerId)
        {
            return await _productRepository.GetProductsBySellerAsync(sellerId);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }
    }
}