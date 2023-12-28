using Microsoft.EntityFrameworkCore;
using QIA.Opc.Domain.Repository;
using Qia.Opc.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QIA.Opc.Infrastructure.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly OpcDbContext _context;

		public GenericRepository(OpcDbContext context)
		{
			_context = context;
		}

		public virtual async Task<T> FindAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>().Where(filter);

			if (asNoTracking)
			{
				query = query.AsNoTracking();
			}

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await query.FirstOrDefaultAsync();
		}

		public virtual async Task<T> AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
			return entity;
		}

		public virtual async Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (asNoTracking)
			{
				query = query.AsNoTracking();
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

			var existingEntity = await _context.Set<T>().FirstOrDefaultAsync(filter);

			if (existingEntity != null)
			{
				// TODO: handle concurrency
				_context.Update(existingEntity);
			}
			else
			{
				await _context.Set<T>().AddAsync(entity);
			}
		}

		public virtual async Task UpdateAsync(T entity)
		{
			if (entity == null) return;

			// TODO: handle concurrency
			_context.Update(entity);
		}

		public virtual async Task DeleteAsync(T entity)
		{
			if (entity == null) return;
			_context.Set<T>().Remove(entity);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
