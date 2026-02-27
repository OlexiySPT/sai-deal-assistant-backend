using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
	public UnitOfWork(DbContext dbContext)
	{
		DbContext = dbContext;
	}

	protected DbContext DbContext { get; }

	public async Task ExecuteResilientTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
	{
		await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
		{
			await using (var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken))
			{
				try
				{
					await action();
					await CommitTransactionAsync(cancellationToken);
				}
				catch
				{
					await RollbackTransactionAsync(cancellationToken);
					throw;
				}
			}
		});
	}

	public async Task<object> BeginTransactionAsync(CancellationToken cancellationToken)
	{
		if (DbContext.Database.CurrentTransaction == null)
		{
			return await DbContext.Database.BeginTransactionAsync(cancellationToken);
		}
		else
		{
			throw new InvalidOperationException(
				"Trying to open new transaction while existing has not been closed yet");
		}
	}

	public async Task CommitTransactionAsync(CancellationToken cancellationToken)
	{
		if (DbContext.Database.CurrentTransaction != null)
		{
			await DbContext.Database.CurrentTransaction.CommitAsync(cancellationToken);
		}
	}

	public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
	{
		if (DbContext.Database.CurrentTransaction != null)
		{
			await DbContext.Database.CurrentTransaction.RollbackAsync(cancellationToken);
		}
	}
}
