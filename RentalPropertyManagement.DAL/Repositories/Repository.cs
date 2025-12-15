using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks; // Cần thêm using này

namespace RentalPropertyManagement.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly RentalDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(RentalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // --- ASYNC IMPLEMENTATION ---

        public async Task<TEntity> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Sử dụng FirstOrDefaultAsync để tìm một đối tượng theo điều kiện
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // --- SYNC IMPLEMENTATION (Giữ nguyên) ---

        public TEntity Get(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsNoTracking();
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}