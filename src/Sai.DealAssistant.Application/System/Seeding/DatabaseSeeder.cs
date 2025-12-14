using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly;

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

	#region Test Data
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
	#endregion

	#region System Data

	#region Enum Tables
	public static IEnumerable<EventType> GetEventTypes()
	{
		return new List<EventType>
		{
			new EventType { Id = 1, Name = "Video call" },
			new EventType { Id = 2, Name = "Phone Call" },
			new EventType { Id = 3, Name = "Messenger chat" },
			new EventType { Id = 4, Name = "Email" },
			new EventType { Id = 5, Name = "Message" },
			new EventType { Id = 6, Name = "Offline meeting" }
		};
	}

	public static IEnumerable<EventState> GetEventStates()
	{
		return new List<EventState>
		{
			new EventState { Id = 1, State = "Planned" },
			new EventState { Id = 2, State = "In Progress" },
			new EventState { Id = 3, State = "Completed" },
			new EventState { Id = 4, State = "Cancelled" },
			new EventState { Id = 5, State = "Deferred" }
		};
	}

	public static IEnumerable<DealState> GetDealStates()
	{
		return new List<DealState>
		{
			new DealState { Id = 1, State = "New" },
			new DealState { Id = 2, State = "Contacted" },
			new DealState { Id = 3, State = "Qualified" },
			new DealState { Id = 4, State = "Proposal" },
			new DealState { Id = 5, State = "Won" },
			new DealState { Id = 6, State = "Lost" }
		};
	}

	public static IEnumerable<DealType> GetDealTypes()
	{
		return new List<DealType>
		{
			new DealType { Id = 1, Name = "One-time Service" },
			new DealType { Id = 2, Name = "Series" },
			new DealType { Id = 3, Name = "Long-time Collaboration" }
		};
	}
	#endregion

	public static IEnumerable<User> GetUsers()
	{
		// NOTE: placeholder password hashes - replace with secure hashes when needed
		return new List<User>
		{
			new User { Id = 1, Username = "admin", PasswordHash = "AQAAAAEAACcQAAAAE-admin-hash", Role = "Administrator" },
			new User { Id = 2, Username = "standard.user", PasswordHash = "AQAAAAEAACcQAAAAE-user-hash", Role = "User" }
		};
	}

	#endregion

	public async Task SeedAsync()
	{
		await _seedRepository.SeedEventTypesAsync(GetEventTypes);
		await _seedRepository.SeedEventStatusesAsync(GetEventStates);
		await _seedRepository.SeedDealStatesAsync(GetDealStates);
		await _seedRepository.SeedDealTypesAsync(GetDealTypes);
		await _seedRepository.SeedUsersAsync(GetUsers);
	}

	public async Task SeedTestDataAsync()
	{
		await _seedRepository.SeedCustomersAsync(GetCustomers);
		await _seedRepository.SeedEmployeesAsync(GetEmployees);
	}
}
