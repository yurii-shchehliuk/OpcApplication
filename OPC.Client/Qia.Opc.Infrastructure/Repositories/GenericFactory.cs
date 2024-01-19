using Microsoft.EntityFrameworkCore;
using Qia.Opc.Persistence;
using QIA.Opc.Domain.Repository;
using System.Linq.Expressions;

namespace QIA.Opc.Infrastructure.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
	{
		private readonly IDbContextFactory<OpcDbContext> _contextFactory;

		public GenericRepository(IDbContextFactory<OpcDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public virtual async Task<T> AddAsync(T entity)
		{
			using var _context = _contextFactory.CreateDbContext();
			await _context.Set<T>().AddAsync(entity);
			await _context.SaveChangesAsync();

			return entity;
		}

		public virtual async Task<T> FindAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
		{
			using var _context = _contextFactory.CreateDbContext();
			var query = _context.Set<T>().Where(filter);

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await query.FirstOrDefaultAsync();
		}

		public virtual async Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
		{
			using var _context = _contextFactory.CreateDbContext();
			IQueryable<T> query = _context.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await query.ToListAsync();
		}

		public virtual async Task UpsertAsync(T entity, Expression<Func<T, bool>> filter)
		{
			if (entity == null) return;

			using var _context = _contextFactory.CreateDbContext();
			var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(filter);

			if (existingEntity != null)
			{
				///TODO: handle conqurency
				_context.Update(existingEntity);
			}
			else
			{
				await _context.Set<T>().AddAsync(entity);
			}

			await _context.SaveChangesAsync();
		}

		public virtual async Task UpdateAsync(T entity)
		{
			if (entity == null) return;

			using var _context = _contextFactory.CreateDbContext();
			///TODO: handle conqurency
			_context.Update(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task DeleteAsync(T entity)
		{
			using var _context = _contextFactory.CreateDbContext();
			if (entity == null) return;

			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task DeleteAsync(Expression<Func<T, bool>> filter)
		{
			using var _context = _contextFactory.CreateDbContext();
			var result = await _context.Set<T>().FirstOrDefaultAsync(filter);

			if (result == null) return;
			_context.Set<T>().Remove(result);
			await _context.SaveChangesAsync();

		}

		public async Task SaveChangesAsync()
		{
			using var _context = _contextFactory.CreateDbContext();
			await _context.SaveChangesAsync();
		}
	}
}
