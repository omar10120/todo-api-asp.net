
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ayagroup_SMS.Core.Interfaces.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        
        Task<T> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetAllAsQueryable();

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);



    }
}
