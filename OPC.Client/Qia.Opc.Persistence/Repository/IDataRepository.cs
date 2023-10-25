using Qia.Opc.Domain.Entities.Interfaces;
using System.Linq.Expressions;

namespace Qia.Opc.Persistence.Repository
{
	public interface IDataRepository<T> where T : class
	{
		Task UpsertAsync(T entity, Expression<Func<T, bool>> filter);
		Task AddAsync(T entity);
		Task DeleteAsync(T entity);
		Task<T> FindAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
		Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null);

		Task SaveChangesAsync();
	}
}