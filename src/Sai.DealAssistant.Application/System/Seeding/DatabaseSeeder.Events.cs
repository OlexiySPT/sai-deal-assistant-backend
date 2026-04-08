using Sai.DealAssistant.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
    public static IEnumerable<Event> GetTestEventsForDeal(Deal deal)
    {
        if (deal is null) throw new ArgumentNullException(nameof(deal));

        // Deterministic seed derived from deal.Name + Id to keep events stable and uniquely dependent on the deal.
        var seed = GetDeterministicSeed($"{deal.Name}-{deal.Id}");
        var rnd = new Random(seed);

        // Realistic event templates (typical sales lifecycle)
        var topics = new[]
        {
            "Discovery call",
            "Qualification",
            "Product demo",
            "Proposal review",
            "Negotiation session",
            "Contract discussion",
            "Onboarding kick-off",
            "Follow-up meeting"
        };

        var agendas = new[]
        {
            "Kickoff / Intro call",
            "Qualification call",
            "Product demo",
            "Proposal presentation",
            "Negotiation",
            "Contract review",
            "Onboarding discussion",
            "Follow-up"
        };

        var results = new[]
        {
            "No decision yet",
            "Interested — follow up",
            "Proposal sent",
            "Negotiation started",
            "Won — contract signed",
            "Lost — not a fit"
        };

        // number of events: 2..7 (deterministic)
        var count = 2 + (seed % 6);

        // Spread events over the last 12 months for realism
        var end = deal.CreatedAt.AddMonths(3);
        var start = end.AddYears(-1);

        // ContactPersons now live on the Firm (loaded via Include in SeedEventsAsync).
        // Guard against deals that have no firm or a firm with no contact persons.
        var contactPersons = deal.Firm?.ContactPersons?.ToArray();

        var events = new List<Event>(count);

        for (var i = 0; i < count; i++)
        {
            // pick a date between start and now, biased so earlier events come first
            var proportion = (double)i / Math.Max(1, count - 1);
            var jitterDays = rnd.NextDouble() * 30.0 - 15.0; // ±15 days jitter
            var daysSpan = (end - start).TotalDays * proportion + jitterDays;
            var date = start.AddDays(Math.Clamp(daysSpan, 0, (end - start).TotalDays))
                            .AddMinutes(rnd.Next(0, 24 * 60));

            var topic = topics[rnd.Next(topics.Length)];
            var agendaTemplate = agendas[rnd.Next(agendas.Length)];
            var resultTemplate = results[rnd.Next(results.Length)];

            // Make agenda/result reference the deal for clarity
            var agenda = $"{agendaTemplate} — {deal.Name}";
            var result = $"{resultTemplate}{(resultTemplate.Contains("Proposal") || resultTemplate.Contains("contract", StringComparison.OrdinalIgnoreCase) ? $" ({deal.Name})" : string.Empty)}";

            // realistic type/state selection (ids seeded elsewhere: types 1..6, states 1..5)
            var typeId  = 1 + rnd.Next(0, 6);  // 1..6
            var stateId = 1 + rnd.Next(0, 5);  // 1..5

            // Deterministically pick a contact person for this event if any exist for the deal's firm.
            // Using the same rnd (seeded by deal) keeps assignment repeatable.
            ContactPerson? chosenContact = null;
            if (contactPersons?.Length > 0)
            {
                chosenContact = contactPersons[rnd.Next(contactPersons.Length)];
            }

            events.Add(new Event
            {
                Pos = i,
                Topic = topic,
                Date = date,
                Agenda = agenda,
                Result = result,
                TypeId = typeId,
                StateId = stateId,
                DealId = deal.Id,
                // Assign ContactPersonId when available; leave null otherwise.
                ContactPersonId = chosenContact?.Id
            });
        }

        // Return events ordered by date to make upserts predictable
        return events.OrderBy(e => e.Date).ToArray();
    }

    private static int GetDeterministicSeed(string key)
    {
        if (string.IsNullOrEmpty(key)) return 0;

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(key);
        var hash = sha.ComputeHash(bytes);

        // Use first 4 bytes as signed int, then get non-negative
        return Math.Abs(BitConverter.ToInt32(hash, 0));
    }
}
