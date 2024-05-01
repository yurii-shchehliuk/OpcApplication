namespace QIA.Opc.Infrastructure.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QIA.Opc.Domain.Repositories;
using QIA.Opc.Infrastructure.DataAccess;

public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
{
    private readonly IDbContextFactory<OpcDbContext> _contextFactory;

    public GenericRepository(IDbContextFactory<OpcDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<T> FindAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        IQueryable<T> query = context.Set<T>().Where(filter);

        if (includes != null)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> filter = null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        IQueryable<T> query = context.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includes != null)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.ToListAsync();
    }

    public virtual async Task UpsertAsync(T entity, Expression<Func<T, bool>> filter)
    {
        if (entity == null)
        {
            return;
        }

        using OpcDbContext context = _contextFactory.CreateDbContext();
        T existingEntity = await context.Set<T>().FirstOrDefaultAsync(filter);

        if (existingEntity != null)
        {
            ///TODO: handle conqurency
            context.Update(existingEntity);
        }
        else
        {
            await context.Set<T>().AddAsync(entity);
        }

        await context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T updatedItem)
    {
        if (updatedItem == null)
        {
            return;
        }

        using OpcDbContext context = _contextFactory.CreateDbContext();
        ///TODO: handle conqurency
        context.Update(updatedItem);
        await context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        if (entity == null)
        {
            return;
        }

        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Expression<Func<T, bool>> filter)
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        T result = await context.Set<T>().FirstOrDefaultAsync(filter);

        if (result == null)
        {
            return;
        }

        context.Set<T>().Remove(result);
        await context.SaveChangesAsync();

    }

    public async Task SaveChangesAsync()
    {
        using OpcDbContext context = _contextFactory.CreateDbContext();
        await context.SaveChangesAsync();
    }
}
