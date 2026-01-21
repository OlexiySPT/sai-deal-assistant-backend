using System;
using System.Collections.Generic;
using System.Linq;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
    public static IEnumerable<DealTag> GetTestDealTags(int dealId)
    {
        // Small realistic pool of tag names — will pick 0..5 per deal.
        var candidateNames = new[]
        {
            "Important",
            "Follow-up",
            "Hot Lead",
            "Low Priority",
            "Negotiation",
            "Ready to Close",
            "Long Term",
            "Needs Approval"
        };

        // Use dealId as seed so tags are deterministic for a given deal.
        var rng = new Random(dealId);

        // Pick a count in range 0..5
        var count = rng.Next(0, 6);

        // Shuffle deterministically and take the first `count` names.
        var chosen = candidateNames
            .OrderBy(_ => rng.Next())
            .Take(count)
            .Select(name => new DealTag
            {
                // Assume DealTag has DealId and Name properties (common seed pattern).
                DealId = dealId,
                Tag = name
            })
            .ToList();

        return chosen;
    }
}
