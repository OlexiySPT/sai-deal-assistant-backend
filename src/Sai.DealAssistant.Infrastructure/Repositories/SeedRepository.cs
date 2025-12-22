using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.Repositories;

public class SeedRepository : ISeedRepository
{
	private readonly ILogger<SeedRepository> _logger;
	private readonly AppDbContext _appDbContext;

	public SeedRepository(
		ILogger<SeedRepository> logger,
		AppDbContext appDbContext)
	{
		_logger = logger is not null ? logger : throw new ArgumentNullException(nameof(logger));
		_appDbContext = appDbContext is not null? appDbContext: throw new ArgumentNullException(nameof(appDbContext));
	}

	public async Task SeedEventTypesAsync(Func<IEnumerable<EventType>> getEventTypes)
	{
		var existing = await _appDbContext.EventTypes.ToListAsync();

		foreach (var et in getEventTypes())
		{
			if (!existing.Exists(e => string.Equals(e.Name, et.Name, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.EventTypes.Add(et);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Event Types Enum table filled.");
	}

	public async Task SeedEventStatusesAsync(Func<IEnumerable<EventState>> getEventStates)
	{
		var existing = await _appDbContext.EventStates.ToListAsync();

		foreach (var st in getEventStates())
		{
			if (!existing.Exists(e => string.Equals(e.State, st.State, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.EventStates.Add(st);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Event Statuses Enum table filled.");
	}

	public async Task SeedDealStatesAsync(Func<IEnumerable<DealState>> getDealStates)
	{
		var existing = await _appDbContext.DealStates.ToListAsync();

		foreach (var st in getDealStates())
		{
			if (!existing.Exists(e => string.Equals(e.State, st.State, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.DealStates.Add(st);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Deal States Enum table filled.");
	}

	public async Task SeedDealTypesAsync(Func<IEnumerable<DealType>> getDealTypes)
	{
		var existing = await _appDbContext.DealTypes.ToListAsync();

		foreach (var t in getDealTypes())
		{
			if (!existing.Exists(e => string.Equals(e.Type, t.Type, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.DealTypes.Add(t);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Deal Types Enum table filled.");
	}

	public async Task SeedDealsAsync(Func<IEnumerable<Deal>> getDeals)
	{
		List<Deal> existing = await _appDbContext.Deals.ToListAsync();

		foreach (Deal d in getDeals())
		{
			if (!existing.Exists(e => string.Equals(e.Name, d.Name, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.Deals.Add(d);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Non-existing Deals created.");
	}

	public async Task SeedUsersAsync(Func<IEnumerable<User>> getUsers)
	{
		var existing = await _appDbContext.Users.ToListAsync();

		foreach (var u in getUsers())
		{
			if (!existing.Exists(e => string.Equals(e.Username, u.Username, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.Users.Add(u);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Users table filled.");
	}
}
