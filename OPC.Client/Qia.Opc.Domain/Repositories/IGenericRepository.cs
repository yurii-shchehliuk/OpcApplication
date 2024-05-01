namespace QIA.Opc.Domain.Repositories;
using System.Linq.Expressions;

public interface IGenericRepository<T> where T : class
{
    Task UpsertAsync(T entity, Expression<Func<T, bool>> filter);
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAsync(Expression<Func<T, bool>> filter);
    Task<T> FindAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
    Task SaveChangesAsync();
    //todo: service for subscription and override
    Task UpdateAsync(T updatedItem);
}
