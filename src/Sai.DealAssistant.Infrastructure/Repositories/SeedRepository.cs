using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly;
using Sai.DealAssistant.Domain.Entities.Samples;
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

	#region Samples
	// ATTENTION This seed is just an example.
	// Use seed only for the entities, which are already predefined (Like countries, User types, etc)
	public async Task SeedCustomersAsync(Func<IEnumerable<SampleCustomer>> getCustomers)
	{
		List<SampleCustomer> customers = await _appDbContext.SampleCustomers.ToListAsync();

		foreach (SampleCustomer customer in getCustomers())
		{
			if (!customers.Exists(c => c.Code == customer.Code))
			{
				_appDbContext.SampleCustomers.Add(customer);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Non-existing Customers created.");
	}

	public async Task SeedEmployeesAsync(Func<IEnumerable<SampleEmployee>> getEmployees)
	{
		var customerIds = _appDbContext.SampleCustomers.OrderBy(p => p.Id).Select(emp => emp.Id).ToList();
		List<SampleEmployee> employees = await _appDbContext.SampleEmployees.ToListAsync();
		var newEmployees = getEmployees();
		int i = 0;
		foreach (var it in newEmployees)
		{
			if (i >= customerIds.Count)
			{
				i = 0;
			}

			it.CustomerId = customerIds[i++];
		}

		foreach (SampleEmployee employee in newEmployees)
		{
			if (!employees.Exists(c => c.Email == employee.Email))
			{
				EntityEntry<SampleEmployee> temp = _appDbContext.SampleEmployees.Add(employee);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Non-existing Employees created.");
	} 
	#endregion

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
			if (!existing.Exists(e => string.Equals(e.Name, t.Name, StringComparison.OrdinalIgnoreCase)))
			{
				_appDbContext.DealTypes.Add(t);
			}
		}

		await _appDbContext.SaveChangesAsync();

		_logger.LogInformation("Deal Types Enum table filled.");
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
