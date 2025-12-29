using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
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
			new DealType { Id = 1, Type = "One-time Service" },
			new DealType { Id = 2, Type = "Series" },
			new DealType { Id = 3, Type = "Long-time Collaboration" }
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
		await _seedRepository.SeedDealsAsync(GetTestDeals);
		// New: seed events after deals so DealId foreign keys are valid
		await _seedRepository.SeedEventsAsync(GetTestEventsForDeal);

    }
}
