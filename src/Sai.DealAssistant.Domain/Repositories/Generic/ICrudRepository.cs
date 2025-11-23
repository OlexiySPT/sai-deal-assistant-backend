using Sai.DealAssistant.Domain.Entities;
using System.Threading.Tasks;

namespace Sai.DealAssistant.Domain.Repositories.Generic
{
	public interface ICrudRepository<TEntity> : IReadRepository<TEntity>
		where TEntity : BaseEntity, new()
	{
		Task<TEntity> CreateAsync(TEntity entity);

		Task<TEntity?> UpdateAsync(TEntity entity);

		Task<TEntity?> DeleteAsync(int id);

		Task<TEntity?> DeleteAsync(TEntity? entity);
	}
}
