using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Domain.Repositories;

public interface ISeedRepository
{
	Task SeedEventTypesAsync(Func<IEnumerable<EventType>> getEventTypes);

	Task SeedEventStatusesAsync(Func<IEnumerable<EventState>> getEventStates);

	Task SeedDealStatesAsync(Func<IEnumerable<DealState>> getDealStates);

	Task SeedDealTypesAsync(Func<IEnumerable<DealType>> getDealTypes);

	Task SeedUsersAsync(Func<IEnumerable<User>> getUsers);

	Task SeedDealsAsync(Func<IEnumerable<Deal>> getDeals);
}
