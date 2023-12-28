using Microsoft.EntityFrameworkCore;
using Qia.Opc.Domain.Entities.Interfaces;
using Qia.Opc.Persistence;
using QIA.Opc.Domain;
using QIA.Opc.Domain.Repository;
using QIA.Opc.Infrastructure.Repositories;

namespace QIA.Opc.Infrastructure
{
	public class UnitOfWork //: IUnitOfWork
	{
		private readonly IDbContextFactory<OpcDbContext> _contextFactory;
		private bool _disposed;
		public UnitOfWork(IDbContextFactory<OpcDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public Task<int> CommitAsync(CancellationToken cancellationToken)
		{
			using var _context = _contextFactory.CreateDbContext();
			return _context.SaveChangesAsync(cancellationToken);
		}

		//public IGenericRepository<T> Repository<T>() where T : class, IBaseEntity, new()
		//{
		//	//return new GenericRepository<T>(_contextFactory);
		//}
	}
}
