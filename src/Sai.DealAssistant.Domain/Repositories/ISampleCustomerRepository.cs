using Sai.DealAssistant.Domain.Entities.Samples;

namespace Sai.DealAssistant.Domain.Repositories;

public interface ISampleCustomerRepository
{
	Task<IReadOnlyCollection<SampleCustomer>> GetAsync();

	Task<SampleCustomer?> GetAsync(string code);

	Task<SampleCustomer?> GetAsync(int id);

	Task<IReadOnlyCollection<SampleCustomer>> SelectAsync(
		string? sortBy,
		bool sortDescending,
		int page,
		int pageSize,
		string? code = null,
		string? name = null);

	Task<int> CountAsync(string? code = null, string? name = null);

	Task<bool> ExistsAsync(string code);

	Task<bool> ExistsAsync(int id);

	Task<SampleCustomer?> CreateAsync(SampleCustomer customer);

	Task<SampleCustomer?> UpdateAsync(SampleCustomer updatedCustomer);
}
