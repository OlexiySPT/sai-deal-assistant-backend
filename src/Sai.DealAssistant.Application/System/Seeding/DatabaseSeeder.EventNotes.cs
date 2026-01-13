using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
	public static IEnumerable<EventNote> GetTestEventNotesForEvent(Event ev)
	{
		if (ev is null) throw new ArgumentNullException(nameof(ev));

		// Create a deterministic seed based on Event.Id and a stable hash of Event.Name
		int seed;
		unchecked
		{
			seed = ev.Id;
			seed = seed * 397 ^ StableStringHash(ev.Agenda ?? string.Empty);
		}
				
		var rng = new Random(seed);

		// Generate a small deterministic number of notes (1..3)
		int count = 1 + rng.Next(3);

		var notes = new List<EventNote>(count);
		for (int i = 1; i <= count; i++)
		{
			notes.Add(new EventNote
			{
				Order = i,
				Text = $"Generated note {i} for event '{ev.Agenda ?? "Unnamed"}'",
				EventId = ev.Id
			});
		}

		return notes;
	}

	private static int StableStringHash(string s)
	{
		// Stable hash implementation (consistent across processes/runs)
		unchecked
		{
			int hash = 23;
			foreach (char c in s)
			{
				hash = hash * 31 + c;
			}
			return hash;
		}
	}
}