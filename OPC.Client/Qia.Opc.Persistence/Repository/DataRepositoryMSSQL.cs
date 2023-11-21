using Microsoft.EntityFrameworkCore;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Qia.Opc.Persistence.Repository
{
	public class DataRepositoryMSSQL<T> : IDataRepository<T> where T : class, new()
	{
		private readonly IDbContextFactory<OpcDbContext> _contextFactory;
		private readonly OpcDbContext _context;

		public DataRepositoryMSSQL(IDbContextFactory<OpcDbContext> context, OpcDbContext opcDbContext)
		{
			_context = opcDbContext;
			_contextFactory = context;
		}

		public async Task<T> FindAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
		{
			try
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
			catch (Exception ex)
			{

				LoggerManager.Logger.Error($"Cannot get the entity: {0}", ex);
			}
			return null;
		}

		public async Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null)
		{
			try
			{
				using var _context = _contextFactory.CreateDbContext();

				IQueryable<T> query = _context.Set<T>();

				if (filter != null)
				{
					query = query.Where(filter);
				}

				return await query.ToListAsync();
			}
			catch (Exception ex)
			{

				LoggerManager.Logger.Error($"Cannot get the entity list: {0}", ex);
			}
			return new List<T>();
		}

		public async Task UpsertAsync(T entity, Expression<Func<T, bool>> filter)
		{
			try
			{
				if (entity == null) return;

				using var context = _contextFactory.CreateDbContext();

				var existingEntity = await context.Set<T>().FirstOrDefaultAsync(filter);

				if (existingEntity != null)
				{
					MapPropertiesExcept(entity, ref existingEntity, "Id", "SessionEntityId");
					///TODO: handle conqurency
					context.Update(existingEntity);
				}
				else
				{
					await context.Set<T>().AddAsync(entity);
				}

				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{

				LoggerManager.Logger.Error($"Cannot update the entity: {0}", ex);
			}
		}

		public async Task DeleteAsync(T entity)
		{
			try
			{
				if (entity == null) return;
				using var _context = _contextFactory.CreateDbContext();
				_context.Set<T>().Remove(entity);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error($"Cannot delete the entity: {0}", ex);
			}
		}

		public async Task AddAsync(T entity)
		{
			try
			{
				using var _context = _contextFactory.CreateDbContext();
				var newEntity = new T();
				MapPropertiesExcept(entity, ref newEntity, "Id");

				await _context.Set<T>().AddAsync(newEntity);
				///TODO: create unit of work
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error($"Cannot add the entity {0}", ex);
			}
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		private void MapPropertiesExcept(T source, ref T destination, params string[] excludedPropertyNames)
		{
			// Get the type of the source/destination
			Type type = typeof(T);

			// Get all properties of the type
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				// Check if this property is one of the excluded properties
				if (!excludedPropertyNames.Contains(property.Name))
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
