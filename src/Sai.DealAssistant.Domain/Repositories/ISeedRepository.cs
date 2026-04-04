using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Domain.Repositories;

public interface ISeedRepository
{
    Task SeedEventTypesAsync(Func<IEnumerable<EventType>> getEventTypes);

    Task SeedEventStatusesAsync(Func<IEnumerable<EventState>> getEventStatuses);

    Task SeedDealStatesAsync(Func<IEnumerable<DealState>> getDealStates);

    Task SeedDealTypesAsync(Func<IEnumerable<DealType>> getDealTypes);

    Task SeedAmountTypesAsync(Func<IEnumerable<AmountType>> getAmountTypes);

    Task SeedUsersAsync(Func<IEnumerable<User>> getUsers);

    /// <summary>
    /// Seeds deals and optionally assigns a firm to each new deal at creation time.
    /// </summary>
    /// <param name="getDeals">Factory that returns the deals to seed.</param>
    /// <param name="assignFirmId">
    /// Optional: given a deal and the list of already-seeded firms, returns the FirmId to assign.
    /// When null, no firm assignment is performed.
    /// </param>
    Task SeedDealsAsync(
        Func<IEnumerable<Deal>> getDeals,
        Func<Deal, IReadOnlyList<Firm>, int?>? assignFirmId = null);

    Task SeedEventsAsync(Func<Deal, IEnumerable<Event>> getEventsForDeal);

    Task SeedFirmContactPersonsAsync(Func<Firm, IEnumerable<ContactPerson>> getContactPersonsForFirm);

    Task SeedDealTagsAsync(Func<int, IEnumerable<DealTag>> getTagsForDeal);

    Task SeedEventNotesAsync(Func<Event, IEnumerable<EventNote>> getNotesForEvent);

    Task<IReadOnlyList<Firm>> SeedFirmsAsync(Func<IEnumerable<Firm>> getFirms);

    Task MultiplyFirmsAsync(int targetRowCount);

    Task MultiplyDealsAsync(int targetRowCount);

    Task MultiplyDealDependentsAsync(int targetRowCount);
    Task<bool> AnyTestDataExistsAsync();
}
