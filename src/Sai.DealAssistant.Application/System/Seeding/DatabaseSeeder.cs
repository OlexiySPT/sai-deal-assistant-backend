using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.System.Seeding;

public class DatabaseSeeder
{
	//private readonly ILogger<DatabaseSeeder> _logger;
	private readonly ISeedRepository _seedRepository;

	public DatabaseSeeder(
		//ILogger<DatabaseSeeder> logger,
		ISeedRepository systemConfigRepository)
	{
		//_logger = logger;
		_seedRepository = systemConfigRepository;
	}

	public static IEnumerable<SampleCustomer> GetCustomers()
	{
		List<SampleCustomer> result = new List<SampleCustomer>
		{
			// Please DO NOT CHANGE this data to not break existing tests, only add new or provide undefined fields
			new SampleCustomer() { Code = "CocaCola", Name = "Coca-Cola", Country = "US", TaxNumber = "CC165299", DateRegistered = new DateTime(1923, 01, 03)},
			new SampleCustomer() { Code = "Pepsi", Name = "Pepsico International", Country = "US", TaxNumber = "P223322", DateRegistered = new DateTime(1947, 03, 03)},
			new SampleCustomer() {Code = "IBM", Name = "International Business Machine", Country = "US", TaxNumber = "IBM165299", DateRegistered = new DateTime(1984, 09, 11)},
			new SampleCustomer() {Code = "Tesla", Name = "Tesla innovations", Country = "US", TaxNumber = "TSL678901", DateRegistered = new DateTime(2012, 07, 14) },
			new SampleCustomer() {Code = "Supermarine", Name = "Supermarine aircraft", Country = "GB", TaxNumber = "SM8989899", DateRegistered = new DateTime(1926, 11, 19) },
			new SampleCustomer() {Code = "BP", Name = "British Petroleum", Country = "GB", TaxNumber = "BP098776", DateRegistered = new DateTime(1919, 09, 01) },
			new SampleCustomer() {Code = "RollsRoyce", Name = "Rolls royce motors", Country = "GB", TaxNumber = "RR123456789", DateRegistered = new DateTime(1933, 12, 02) },
			new SampleCustomer() {Code = "PZL", Name = "Polskie Zaklady Lotnicze", Country = "Poland", TaxNumber = "PZL5678920", DateRegistered = new DateTime(1928, 07, 30) }
        };
		return result;
	}

	public static IEnumerable<SampleEmployee> GetEmployees()
	{
		List<SampleEmployee> result = new List<SampleEmployee>
		{
			// Please DO NOT CHANGE this data to not break existing tests, only add new or provide undefined fields
			new SampleEmployee { Email = "Mechislaw.Medwecky@mail.dat", FirstName = "Mechislaw", LastName = "Medwecky" },
			new SampleEmployee { Email = "ReginaldJ.Mitchell@mail.dat", FirstName = "ReginaldJ", LastName = "Mitchell" },
			new SampleEmployee { Email = "Jimmy.Doolitle@mail.dat", FirstName = "Jimmy", LastName = "Doolitle" },
			new SampleEmployee { Email = "Nikolay.Polikarpov@mail.dat", FirstName = "Nikolay", LastName = "Polikarpov" },
			new SampleEmployee { Email = "Leroy.Grumman@mail.dat", FirstName = "Leroy", LastName = "Grumman" },
			new SampleEmployee { Email = "Kurt.Tank@mail.dat", FirstName = "Kurt", LastName = "Tank" },
			new SampleEmployee { Email = "Sydney.Camm@mail.dat", FirstName = "Sydney", LastName = "Camm" },
			new SampleEmployee { Email = "Valter.Novotny@mail.dat", FirstName = "Valter", LastName = "Novotny" },
			new SampleEmployee { Email = "Rudolph.Novotny@mail.dat", FirstName = "Rudolph", LastName = "Novotny" },
			new SampleEmployee { Email = "Kade.Cooper@mail.dat", FirstName = "Kade", LastName = "Cooper" },
			new SampleEmployee { Email = "Katerina.Larsen@mail.dat", FirstName = "Katerina", LastName = "Larsen" },
			new SampleEmployee { Email = "Scarlet.Pugh@mail.dat", FirstName = "Scarlet", LastName = "Pugh" },
			new SampleEmployee { Email = "Louis.Welch@mail.dat", FirstName = "Louis", LastName = "Welch" },
			new SampleEmployee { Email = "Rachel.Stephens@mail.dat", FirstName = "Rachel", LastName = "Stephens" },
			new SampleEmployee { Email = "Elysia.Sharpe@mail.dat", FirstName = "Elysia", LastName = "Sharpe" },
			new SampleEmployee { Email = "Harris.Stevens@mail.dat", FirstName = "Harris", LastName = "Stevens" },
			new SampleEmployee { Email = "Jenny.Osborne@mail.dat", FirstName = "Jenny", LastName = "Osborne" },
			new SampleEmployee { Email = "Mohammad.Beard@mail.dat", FirstName = "Mohammad", LastName = "Beard" },
			new SampleEmployee { Email = "Ashwin.Robertson@mail.dat", FirstName = "Ashwin", LastName = "Robertson" },
		};

		return result;
	}

	public async Task SeedAsync()
	{
		await Task.FromResult(0);
	}

	public async Task SeedTestDataAsync()
	{
		await _seedRepository.SeedCustomersAsync(GetCustomers);
		await _seedRepository.SeedEmployeesAsync(GetEmployees);
	}
}
