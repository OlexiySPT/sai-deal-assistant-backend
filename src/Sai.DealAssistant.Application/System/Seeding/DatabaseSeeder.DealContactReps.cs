// Sai.DealAssistant.Application\System\Seeding\DatabaseSeeder.DealContactReps.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
	public static IEnumerable<DealContactRep> GetTestContactRepsForDeal(Deal deal)
	{
		if (deal is null) throw new ArgumentNullException(nameof(deal));

		// Deterministic seed derived from deal.Name to keep reps stable and uniquely dependent on the deal.
		var seed = GetDeterministicSeed(deal.Name);
		var rnd = new Random(seed);

		var firstNames = new[] { "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Riley", "Sam", "Jamie", "Avery", "Drew", "Pat", "Lee" };
		var lastNames = new[] { "Smith", "Johnson", "Brown", "Williams", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson" };
		var positions = new[] { "CTO", "Head of Product", "VP Sales", "Project Manager", "Director", "Account Executive", "Technical Lead", "Business Analyst", "Operations Manager", "Customer Success Manager" };
		var domains = new[] { "example.com", "acme.example.com", "contoso.example.com", "globex.example.com", "examplecorp.com" };
		var descriptions = new[]
		{
			"Primary contact for procurement and contracting.",
			"Technical contact responsible for integration.",
			"Business stakeholder with decision authority.",
			"Support contact for onboarding and rollout.",
			"Key sponsor for the project."
		};

		// deterministically choose number of reps: 1..10
		var count = 1 + (seed % 10);

		var reps = new List<DealContactRep>(count);

		// Ensure variety but deterministic ordering
		for (var i = 0; i < count; i++)
		{
			var first = firstNames[rnd.Next(firstNames.Length)];
			var last = lastNames[rnd.Next(lastNames.Length)];
			var pos = positions[rnd.Next(positions.Length)];
			var domain = domains[rnd.Next(domains.Length)];
			var desc = descriptions[rnd.Next(descriptions.Length)];

			var name = $"{first} {last}";
			// create a stable but unique email per contact for this deal
			var email = $"{first.ToLower()}.{last.ToLower()}{(seed % 100)}{i}@{domain}";
			// phone in realistic NANP format with deterministic values
			var phone = $"+1-555-{rnd.Next(200, 999):D3}-{rnd.Next(1000, 9999):D4}";

			reps.Add(new DealContactRep
			{
				Name = name,
				Position = pos,
				Email = email,
				Phone = phone,
				Description = $"{desc} — {deal.Name}",
				DealId = deal.Id
			});
		}

		// Return ordered by Name for predictability
		return reps.OrderBy(r => r.Name).ToArray();
	}
}