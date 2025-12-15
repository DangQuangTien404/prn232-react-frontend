using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks; // Cần thêm using này

namespace RentalPropertyManagement.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // --- PHƯƠNG THỨC BẤT ĐỒNG BỘ (ASYNC) ---
        Task<TEntity> GetAsync(int id);

        // Phương thức quan trọng cho Login/Register (Tìm User theo Email)
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);

        // --- PHƯƠNG THỨC ĐỒNG BỘ (SYNC) ---
        // Giữ lại các phương thức đồng bộ hiện có (nhưng nên chuyển hết sang Async)
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        // Lưu ý: Tôi đang giữ lại các phương thức Sync để tránh gây thêm lỗi nếu chúng được dùng ở nơi khác
    }
}