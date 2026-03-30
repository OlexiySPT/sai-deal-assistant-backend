using Sai.DealAssistant.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
	public static IEnumerable<Event> GetTestEventsForDeal(Deal deal)
    {
        if (deal is null) throw new ArgumentNullException(nameof(deal));

        var seed = GetDeterministicSeed($"{deal.Name}-{deal.Id}");
        var rnd = new Random(seed);

        // Realistic event templates (typical sales lifecycle)
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

        var topics = new[]
        {
            "Initial contact and discovery",
            "Requirements gathering",
            "Solution walkthrough",
            "Commercial terms discussion",
            "Scope and timeline alignment",
            "Technical deep-dive",
            "Contract and legal review",
            "Post-sales handover"
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

        var count = 2 + (seed % 6);

        var end = deal.CreatedAt.AddMonths(3);
        var start = end.AddYears(-1);

        var contactPersons = deal.ContactPersons?.ToArray();

        var events = new List<Event>(count);

        for (var i = 0; i < count; i++)
        {
            var proportion = (double)i / Math.Max(1, count - 1);
            var jitterDays = rnd.NextDouble() * 30.0 - 15.0;
            var daysSpan = (end - start).TotalDays * proportion + jitterDays;
            var date = start.AddDays(Math.Clamp(daysSpan, 0, (end - start).TotalDays))
                            .AddMinutes(rnd.Next(0, 24 * 60));

            var agendaTemplate = agendas[rnd.Next(agendas.Length)];
            var topicTemplate = topics[rnd.Next(topics.Length)];
            var resultTemplate = results[rnd.Next(results.Length)];

            var agenda = $"{agendaTemplate} — {deal.Name}";
            var topic = $"{topicTemplate} — {deal.Name}";
            var result = $"{resultTemplate}{(resultTemplate.Contains("Proposal") || resultTemplate.Contains("contract", StringComparison.OrdinalIgnoreCase) ? $" ({deal.Name})" : string.Empty)}";

            var typeId = 1 + rnd.Next(0, 6);
            var stateId = 1 + rnd.Next(0, 5);

            ContactPerson? chosenContact = null;
            if (contactPersons?.Length > 0)
            {
                chosenContact = contactPersons[rnd.Next(contactPersons.Length)];
            }

            events.Add(new Event
            {
                Pos = i,
                Date = date,
                Topic = topic,
                Agenda = agenda,
                Result = result,
                TypeId = typeId,
                StateId = stateId,
                DealId = deal.Id,
                ContactPersonId = chosenContact?.Id
            });
        }

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


