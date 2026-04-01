using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
    public static IEnumerable<Firm> GetTestFirms()
    {
        return new List<Firm>
        {
            new Firm { Name = "Acme Corp", Country = "USA", Description = "Global leader in diversified consumer products." },
            new Firm { Name = "Contoso Ltd", Country = "UK", Description = "Enterprise software and managed services provider." },
            new Firm { Name = "Globex Industries", Country = "Germany", Description = "Heavy manufacturing and industrial automation." },
            new Firm { Name = "Initech Solutions", Country = "Canada", Description = "IT consulting and systems integration." },
            new Firm { Name = "Umbrella Technologies", Country = "France", Description = "Healthcare technology and biomedical research." },
            new Firm { Name = "Stark Innovations", Country = "USA", Description = "Advanced engineering and aerospace R&D." },
            new Firm { Name = "BlueSun Ventures", Country = "Australia", Description = "Renewable energy and sustainability programs." },
            new Firm { Name = "Quantum Dynamics", Country = "Netherlands", Description = "Data science and AI-driven analytics platforms." },
            new Firm { Name = "Helios Systems", Country = "Spain", Description = "Cloud infrastructure and managed hosting." },
            new Firm { Name = "Apex Digital", Country = "Poland", Description = "Cybersecurity consulting and penetration testing." },
        };
    }

    public static int? AssignFirmToDeal(Deal deal, IReadOnlyList<Firm> seededFirms)
    {
        if (seededFirms.Count == 0) return null;

        // Deterministically assign a firm to every deal.
        var seed = GetDeterministicSeed($"firm-{deal.Name}");
        return seededFirms[seed % seededFirms.Count].Id;
    }
}