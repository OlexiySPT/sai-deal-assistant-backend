using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Domain.Repositories;

public interface ISeedRepository
{
	Task SeedCustomersAsync(Func<IEnumerable<SampleCustomer>> getCustomers);

	Task SeedEmployeesAsync(Func<IEnumerable<SampleEmployee>> getEmployees);
}
