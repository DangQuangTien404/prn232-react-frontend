using EShop.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EShopDbContext _context;

        public ProductRepository(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetProductsBySellerAsync(int sellerId)
        {
            return await _context.Products
                .Where(p => p.SellerId == sellerId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> GetAllAsync()
        {

            return await _context.Products.Include(p => p.Seller).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}