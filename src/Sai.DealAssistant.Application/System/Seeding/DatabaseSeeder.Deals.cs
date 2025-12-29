using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
    public static IEnumerable<Deal> Deals
    {
        get
        {
            // A small set of test deals. Keep values stable to avoid breaking tests.
            List<Deal> result = new List<Deal>
            {
                new Deal
                {
                    Name = "Acme Website Revamp",
                    Description = "One-time website redesign and optimization",
                    Url = "https://acme.example.com",
                    AiSearchInfo = "website, redesign, SEO",
                    AiBriefDescription = "Redesign front-end, improve conversion",
                    Industry = "Marketing",
                    Status = "Open",
                    TypeId = 1, // One-time Service
				    StateId = 1 // New
			    },
                new Deal
                {
                    Name = "Contoso Monthly Support",
                    Description = "Ongoing monthly support subscription",
                    Url = "https://contoso.example.com/support",
                    AiSearchInfo = "support, subscription, SLA",
                    AiBriefDescription = "Monthly support and maintenance",
                    Industry = "IT Services",
                    Status = "Active",
                    TypeId = 2, // Series
				    StateId = 2 // Contacted
			    },
                new Deal
                {
                    Name = "Globex Research Collaboration",
                    Description = "Long-term R&D partnership",
                    Url = "https://globex.example.com",
                    AiSearchInfo = "research, collaboration, R&D",
                    AiBriefDescription = "Multi-year collaboration on new products",
                    Industry = "Manufacturing",
                    Status = "Negotiation",
                    TypeId = 3, // Long-time Collaboration
				    StateId = 4 // Proposal
			    },
                new Deal
                {
                    Name = "Initech Prototype",
                    Description = "Prototype development engagement",
                    Url = "https://initech.example.com",
                    AiSearchInfo = "prototype, MVP",
                    AiBriefDescription = "Build MVP for product validation",
                    Industry = "Software",
                    Status = "Closed - Won",
                    TypeId = 1,
                    StateId = 5
                },
                new Deal
                {
                    Name = "Umbrella Maintenance",
                    Description = "Periodic maintenance for legacy systems",
                    Url = "https://umbrella.example.com",
                    AiSearchInfo = "maintenance, legacy",
                    AiBriefDescription = "Scheduled maintenance and updates",
                    Industry = "Healthcare",
                    Status = "Closed - Lost",
                    TypeId = 2,
                    StateId = 6
                },
                new Deal
                {
                    Name = "Stark Labs Integration",
                    Description = "Integration of 3rd party APIs",
                    Url = "https://stark.example.com",
                    AiSearchInfo = "integration, APIs",
                    AiBriefDescription = "Integrate external services",
                    Industry = "Engineering",
                    Status = "In Progress",
                    TypeId = 2,
                    StateId = 3
                },
                new Deal
                {
                    Name = "Quantum Analytics Pilot",
                    Description = "Short pilot to evaluate quantum-assisted analytics",
                    Url = "https://quantum.example.com",
                    AiSearchInfo = "quantum, analytics, pilot",
                    AiBriefDescription = "Pilot analytics using quantum-inspired models",
                    Industry = "Data Science",
                    Status = "Open",
                    TypeId = 1,
                    StateId = 1
                },
                new Deal
                {
                    Name = "BlueSun Digital Transformation",
                    Description = "Company-wide digital transformation program",
                    Url = "https://bluesun.example.com",
                    AiSearchInfo = "digital transformation, cloud, modernization",
                    AiBriefDescription = "Platform migration and process automation",
                    Industry = "Retail",
                    Status = "Active",
                    TypeId = 3,
                    StateId = 2
                },
                new Deal
                {
                    Name = "Greenfield Mobile App",
                    Description = "Design and build a new mobile application",
                    Url = "https://greenfield.example.com",
                    AiSearchInfo = "mobile app, iOS, Android, UX",
                    AiBriefDescription = "End-to-end mobile product development",
                    Industry = "Fintech",
                    Status = "Proposal",
                    TypeId = 1,
                    StateId = 4
                },
                new Deal
                {
                    Name = "Helios Cloud Migration",
                    Description = "Migrate legacy services to cloud infrastructure",
                    Url = "https://helios.example.com",
                    AiSearchInfo = "cloud migration, AWS, Azure, lift-and-shift",
                    AiBriefDescription = "Rehost and optimize services in cloud",
                    Industry = "Telecom",
                    Status = "In Progress",
                    TypeId = 2,
                    StateId = 3
                },
                new Deal
                {
                    Name = "Apex Security Assessment",
                    Description = "Comprehensive security audit and remediation plan",
                    Url = "https://apex.example.com",
                    AiSearchInfo = "security, audit, pentest",
                    AiBriefDescription = "Identify vulnerabilities and remediation",
                    Industry = "Banking",
                    Status = "Open",
                    TypeId = 1,
                    StateId = 1
                },
                new Deal
                {
                    Name = "Vertex AI Chatbot",
                    Description = "AI chatbot for customer support automation",
                    Url = "https://vertex.example.com",
                    AiSearchInfo = "chatbot, ai, nlp, support",
                    AiBriefDescription = "Conversational assistant for support workflows",
                    Industry = "Customer Service",
                    Status = "Active",
                    TypeId = 2,
                    StateId = 2
                },
                new Deal
                {
                    Name = "Orion Data Warehouse",
                    Description = "Build centralized data warehouse and ETL pipelines",
                    Url = "https://orion.example.com",
                    AiSearchInfo = "data warehouse, ETL, analytics",
                    AiBriefDescription = "Centralize analytics-ready data",
                    Industry = "Analytics",
                    Status = "Negotiation",
                    TypeId = 3,
                    StateId = 4
                },
                new Deal
                {
                    Name = "Nimbus IoT Deployment",
                    Description = "Deploy IoT sensors and backend for monitoring",
                    Url = "https://nimbus.example.com",
                    AiSearchInfo = "IoT, sensors, telemetry",
                    AiBriefDescription = "Edge devices with cloud telemetry",
                    Industry = "Manufacturing",
                    Status = "In Progress",
                    TypeId = 2,
                    StateId = 3
                },
                new Deal
                {
                    Name = "Cascade Accessibility Upgrade",
                    Description = "Improve accessibility across web properties",
                    Url = "https://cascade.example.com",
                    AiSearchInfo = "accessibility, WCAG, a11y",
                    AiBriefDescription = "Make websites accessible to all users",
                    Industry = "Public Sector",
                    Status = "Proposal",
                    TypeId = 1,
                    StateId = 4
                },
                new Deal
                {
                    Name = "Atlas Localization Program",
                    Description = "Localization of product for APAC markets",
                    Url = "https://atlas.example.com",
                    AiSearchInfo = "localization, i18n, translations",
                    AiBriefDescription = "Localization, compliance, and regional launch",
                    Industry = "Software",
                    Status = "Active",
                    TypeId = 3,
                    StateId = 2
                }
            };

            return result;
        }
    }
}
