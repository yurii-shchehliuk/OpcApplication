using QIA.Opc.Domain.Entities;
using System.Linq.Expressions;

namespace QIA.Opc.Domain.Repository
{
	public interface IGenericRepository<T> where T : class
	{
		Task UpsertAsync(T entity, Expression<Func<T, bool>> filter);
		Task<T> AddAsync(T entity);
		Task DeleteAsync(T entity);
		Task<T> FindAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
		Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
		Task SaveChangesAsync();
		Task UpdateAsync(T subsUpdated);
	}
}