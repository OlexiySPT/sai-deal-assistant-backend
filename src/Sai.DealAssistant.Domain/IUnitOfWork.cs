namespace Sai.DealAssistant.Domain;

public interface IUnitOfWork
{
	Task ExecuteResilientTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

	Task<object> BeginTransactionAsync(CancellationToken cancellationToken = default);

	Task CommitTransactionAsync(CancellationToken cancellationToken = default);

	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
