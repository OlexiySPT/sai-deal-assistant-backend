using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
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
	public async Task SeedEventTypeAsync()
    {
        var eventTypes = await _appDbContext.EventTypes.ToListAsync();

        _appDbContext.EventTypes.Add(new EventType { Id = 1, Name = "Video call", Description = "Video call via Teams or Google meet,etc" });
        _appDbContext.EventTypes.Add(new EventType { Id = 2, Name = "Phone Call", Description="Call by telephone, not by messenger"});
        _appDbContext.EventTypes.Add(new EventType { Id = 3, Name = "Messenger chat", Description = "Chat in messenger or social network" });
        _appDbContext.EventTypes.Add(new EventType { Id = 4, Name = "Email", Description = "Chat in messenger or social network" });
        _appDbContext.EventTypes.Add(new EventType { Id = 5, Name = "Message", Description = "Message by messenger or SMS" });
        _appDbContext.EventTypes.Add(new EventType { Id = 6, Name = "Offline meeting", Description = "Meeting with client" });

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Non-existing Customers created.");
    }
}
