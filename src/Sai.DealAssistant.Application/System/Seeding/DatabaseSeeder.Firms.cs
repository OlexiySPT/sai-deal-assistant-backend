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
            new Firm { Name = "NovaTech Partners", Country = "Sweden", Description = "Software product engineering and agile delivery." },
            new Firm { Name = "Meridian Capital", Country = "USA", Description = "Private equity and venture capital investments." },
            new Firm { Name = "Pinnacle Logistics", Country = "Germany", Description = "Supply chain management and freight forwarding." },
            new Firm { Name = "Crestview Analytics", Country = "UK", Description = "Business intelligence and market research." },
            new Firm { Name = "Orion Financial", Country = "Switzerland", Description = "Wealth management and institutional banking." },
            new Firm { Name = "Solaris Energy", Country = "Italy", Description = "Solar power generation and grid integration." },
            new Firm { Name = "Nexus Biomedical", Country = "USA", Description = "Clinical diagnostics and medical device manufacturing." },
            new Firm { Name = "Atlas Infrastructure", Country = "Brazil", Description = "Civil engineering and large-scale construction projects." },
            new Firm { Name = "Cascade Software", Country = "Canada", Description = "ERP solutions and enterprise workflow automation." },
            new Firm { Name = "Ironclad Security", Country = "Israel", Description = "Network security and threat intelligence services." },
            new Firm { Name = "Vega Retail Group", Country = "France", Description = "Omnichannel retail and e-commerce operations." },
            new Firm { Name = "Synapse AI", Country = "USA", Description = "Machine learning infrastructure and MLOps platforms." },
            new Firm { Name = "Horizon Marine", Country = "Norway", Description = "Offshore oil and gas engineering services." },
            new Firm { Name = "TerraFirm Agriculture", Country = "Argentina", Description = "Precision agriculture and agri-tech solutions." },
            new Firm { Name = "Cobalt Media", Country = "UK", Description = "Digital advertising and programmatic media buying." },
            new Firm { Name = "Ember Fintech", Country = "Singapore", Description = "Payment processing and digital banking infrastructure." },
            new Firm { Name = "Polaris Defense", Country = "USA", Description = "Defense contracting and military systems integration." },
            new Firm { Name = "Verdant Pharma", Country = "Ireland", Description = "Generic pharmaceuticals and drug formulation." },
            new Firm { Name = "Luminary Studios", Country = "USA", Description = "Game development and interactive entertainment." },
            new Firm { Name = "FrontLine Robotics", Country = "Japan", Description = "Industrial robotics and factory automation." },
            new Firm { Name = "Sterling Aerospace", Country = "UK", Description = "Satellite communications and space systems." },
            new Firm { Name = "Delphi Consulting", Country = "Greece", Description = "Management consulting and organizational transformation." },
            new Firm { Name = "Titan Chemical", Country = "Germany", Description = "Specialty chemicals and polymer manufacturing." },
            new Firm { Name = "Argo Shipping", Country = "Denmark", Description = "Container shipping and port logistics." },
            new Firm { Name = "Radiant Healthcare", Country = "Canada", Description = "Hospital information systems and patient management." },
            new Firm { Name = "Clockwork Automation", Country = "Switzerland", Description = "Process automation and precision instrumentation." },
            new Firm { Name = "Evergreen Capital", Country = "Hong Kong", Description = "Asset management and hedge fund operations." },
            new Firm { Name = "Prism Optics", Country = "Japan", Description = "Photonics, laser systems, and optical components." },
            new Firm { Name = "Thunderbolt Networks", Country = "USA", Description = "Telecommunications infrastructure and ISP services." },
            new Firm { Name = "Caspian Resources", Country = "Kazakhstan", Description = "Mining and natural resource extraction." },
            new Firm { Name = "Marble Arch Properties", Country = "UK", Description = "Commercial real estate development and management." },
            new Firm { Name = "Velox Automotive", Country = "Germany", Description = "Electric vehicle components and powertrain systems." },
            new Firm { Name = "Zenith Insurance", Country = "USA", Description = "Commercial insurance underwriting and risk management." },
            new Firm { Name = "Arctic Exploration", Country = "Finland", Description = "Environmental research and polar expedition support." },
            new Firm { Name = "Sapphire Data", Country = "India", Description = "Big data engineering and cloud data warehousing." },
            new Firm { Name = "Beacon Education", Country = "USA", Description = "EdTech platforms and corporate learning solutions." },
            new Firm { Name = "Redwood Timber", Country = "Canada", Description = "Sustainable forestry and wood products manufacturing." },
            new Firm { Name = "Equinox Textiles", Country = "Bangladesh", Description = "Textile manufacturing and apparel sourcing." },
            new Firm { Name = "Odyssey Travel", Country = "Spain", Description = "Luxury travel management and concierge services." },
            new Firm { Name = "Phoenix Semiconductor", Country = "Taiwan", Description = "Chip design, fabrication, and IC packaging." },
            new Firm { Name = "Landmark Legal", Country = "UK", Description = "Corporate law, M&A advisory, and compliance services." },
            new Firm { Name = "Ironwood Mining", Country = "South Africa", Description = "Gold and platinum mining operations." },
            new Firm { Name = "Cascade Brewing", Country = "Belgium", Description = "Craft beer production and distribution." },
            new Firm { Name = "Pulse Medical", Country = "USA", Description = "Telemedicine platforms and remote patient monitoring." },
            new Firm { Name = "NorthStar Aviation", Country = "Canada", Description = "Regional aviation and aircraft maintenance services." },
            new Firm { Name = "Vertex Construction", Country = "UAE", Description = "High-rise construction and urban development." },
            new Firm { Name = "Indigo Publishing", Country = "USA", Description = "Digital publishing and content distribution." },
            new Firm { Name = "SilkRoad Logistics", Country = "China", Description = "Cross-border e-commerce logistics and customs brokerage." },
            new Firm { Name = "Fusion Biotech", Country = "USA", Description = "Gene therapy research and biopharmaceutical development." },
            new Firm { Name = "Coastal Energy", Country = "Mexico", Description = "Wind power generation and offshore energy projects." },
            new Firm { Name = "Granite Financial", Country = "USA", Description = "Commercial lending and structured finance." },
            new Firm { Name = "Neon Studios", Country = "South Korea", Description = "Mobile gaming and social entertainment platforms." },
            new Firm { Name = "Sigma Telecommunications", Country = "Portugal", Description = "Fixed-line and mobile telecommunications operator." },
            new Firm { Name = "Enviro Solutions", Country = "Sweden", Description = "Environmental remediation and waste management." },
            new Firm { Name = "Harborview Shipping", Country = "Greece", Description = "Bulk carrier operations and maritime logistics." },
            new Firm { Name = "Lynx Analytics", Country = "USA", Description = "Predictive analytics and decision intelligence software." },
            new Firm { Name = "Summit Architecture", Country = "Austria", Description = "Architectural design and urban planning consultancy." },
            new Firm { Name = "Cobalt Blue Finance", Country = "Luxembourg", Description = "Investment fund administration and custody services." },
            new Firm { Name = "Crestfall Textiles", Country = "Turkey", Description = "Cotton and synthetic fabric weaving and dyeing." },
            new Firm { Name = "Hyperion Cloud", Country = "USA", Description = "Multi-cloud management and cost optimization." },
            new Firm { Name = "Magellan Exploration", Country = "Australia", Description = "Mineral exploration and geological surveying." },
            new Firm { Name = "Solstice Agriculture", Country = "Ukraine", Description = "Grain trading and agricultural commodity brokerage." },
            new Firm { Name = "Trident Defence", Country = "UK", Description = "Naval systems and underwater technology." },
            new Firm { Name = "Amber Retail", Country = "Russia", Description = "Grocery retail chain and private-label food brands." },
            new Firm { Name = "Opal Healthcare", Country = "Australia", Description = "Aged care facilities and home health services." },
            new Firm { Name = "Wavefront Communications", Country = "USA", Description = "Satellite broadband and rural connectivity." },
            new Firm { Name = "Irongate Capital", Country = "USA", Description = "Mergers and acquisitions advisory and restructuring." },
            new Firm { Name = "Celeste Cosmetics", Country = "Italy", Description = "Luxury beauty products and fragrance development." },
            new Firm { Name = "Thunder Ridge Mining", Country = "Chile", Description = "Copper and lithium mining for battery supply chains." },
            new Firm { Name = "Aqua Systems", Country = "Netherlands", Description = "Water treatment and desalination plant engineering." },
            new Firm { Name = "Centaur Logistics", Country = "India", Description = "Last-mile delivery and warehousing solutions." },
            new Firm { Name = "Valiant Cybersecurity", Country = "USA", Description = "Zero-trust architecture and endpoint protection." },
            new Firm { Name = "Nimbus Fintech", Country = "UK", Description = "Open banking APIs and regulatory compliance technology." },
            new Firm { Name = "Coda Music", Country = "Sweden", Description = "Music streaming technology and rights management." },
            new Firm { Name = "Solace Insurance", Country = "Ireland", Description = "Life insurance products and actuarial services." },
            new Firm { Name = "BluePeak Capital", Country = "USA", Description = "Growth equity and late-stage startup investments." },
            new Firm { Name = "Helix Genomics", Country = "USA", Description = "Next-generation sequencing and genomic data analysis." },
            new Firm { Name = "Amber Waves Energy", Country = "USA", Description = "Natural gas production and pipeline infrastructure." },
            new Firm { Name = "Vantage Point AI", Country = "Canada", Description = "Computer vision and autonomous systems research." },
            new Firm { Name = "Crimson Robotics", Country = "Germany", Description = "Surgical robotics and minimally invasive systems." },
            new Firm { Name = "Pacific Rim Trading", Country = "Japan", Description = "Commodity trading and import/export brokerage." },
            new Firm { Name = "Stratosphere Drones", Country = "USA", Description = "Commercial drone delivery and aerial survey services." },
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