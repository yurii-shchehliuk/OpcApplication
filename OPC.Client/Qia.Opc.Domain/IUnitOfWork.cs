using Qia.Opc.Domain.Entities.Interfaces;
using QIA.Opc.Domain.Repository;

namespace QIA.Opc.Domain
{
	public interface IUnitOfWork
	{
		IGenericRepository<T> Repository<T>() where T : class, IBaseEntity, new();
		Task<int> CommitAsync(CancellationToken cancellationToken);
	}
}
