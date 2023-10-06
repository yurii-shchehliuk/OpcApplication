using Microsoft.EntityFrameworkCore;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Qia.Opc.Persistence.Repository
{
	public class DataRepository<T> : IDataRepository<T> where T : class, new()
	{
		private readonly IDbContextFactory<OpcDbContext> _contextFactory;

		public DataRepository(IDbContextFactory<OpcDbContext> context)
		{
			_contextFactory = context;
		}

		public async Task<T> FindAsync(Expression<Func<T, bool>> filter)
		{
			using var _context = _contextFactory.CreateDbContext();
			return await _context.Set<T>().Where(filter).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<T>> ListAllAsync()
		{
			using var _context = _contextFactory.CreateDbContext();
			var result = await _context.Set<T>().ToListAsync();
			return result;
		}

		public async Task UpsertAsync(T entity, Expression<Func<T, bool>> filter)
		{
			using var context = _contextFactory.CreateDbContext();

			var existingEntity = await context.Set<T>().FirstOrDefaultAsync(filter);

			if (existingEntity != null)
			{
				MapPropertiesExcept<T>(entity, ref existingEntity, "Id");
				///TODO: handle conqurency
				context.Update(existingEntity);
			}
			else
			{
				await context.Set<T>().AddAsync(entity);
			}

			await context.SaveChangesAsync();
		}

		public async Task DeleteAsync(T entity)
		{
			if (entity == null) return;
			using var _context = _contextFactory.CreateDbContext();
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
		}

		public async Task AddAsync(T entity)
		{
			using var _context = _contextFactory.CreateDbContext();
			await _context.Set<T>().AddAsync(entity);
			///TODO: create unit of work
			await _context.SaveChangesAsync();
		}

		private void MapPropertiesExcept<T>(T source, ref T destination, string excludedPropertyName)
		{
			// Get the type of the source/destination
			Type type = typeof(T);

			// Get all properties of the type
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				// Check if this property is the one to exclude
				if (property.Name != excludedPropertyName)
				{
					// Check if the property can be written to
					if (property.CanWrite)
					{
						// Get the value from the source
						object value = property.GetValue(source);

						// Set the value on the destination
						property.SetValue(destination, value);
					}
				}
			}
		}

	}
}
