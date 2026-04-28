using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.System.Seeding;

public partial class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly ISeedRepository _seedRepository;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        ISeedRepository seedRepository)
    {
        _logger = logger;
        _seedRepository = seedRepository;
    }

    #region System Data

    #region Enum Tables
    public static IEnumerable<EventType> GetEventTypes()
    {
        return new List<EventType>
        {
            new EventType { Id = 1, Name = "Video call" },
            new EventType { Id = 2, Name = "Phone Call" },
            new EventType { Id = 3, Name = "Messenger chat" },
            new EventType { Id = 4, Name = "Email" },
            new EventType { Id = 5, Name = "Message" },
            new EventType { Id = 6, Name = "Offline meeting" }
        };
    }

    public static IEnumerable<EventState> GetEventStates()
    {
        return new List<EventState>
        {
            new EventState { Id = 1, State = "Planned" },
            new EventState { Id = 2, State = "In Progress" },
            new EventState { Id = 3, State = "Completed" },
            new EventState { Id = 4, State = "Cancelled" },
            new EventState { Id = 5, State = "Deferred" }
        };
    }

    public static IEnumerable<DealState> GetDealStates()
    {
        return new List<DealState>
        {
            new DealState { Id = 1, State = "New" },
            new DealState { Id = 2, State = "Contacted" },
            new DealState { Id = 3, State = "Tech Qualified" },
            new DealState { Id = 4, State = "Management Qualified" },
            new DealState { Id = 5, State = "Final Qualified" },
            new DealState { Id = 6, State = "Won" },
            new DealState { Id = 7, State = "Lost" }
        };
    }

    public static IEnumerable<DealType> GetDealTypes()
    {
        return new List<DealType>
        {
            new DealType { Id = 1, Type = "One-time Service" },
            new DealType { Id = 2, Type = "Short-term Contract" },
            new DealType { Id = 3, Type = "Long-time Collaboration" }
        };
    }

    public static IEnumerable<AmountType> GetAmountTypes()
    {
        return new List<AmountType>
        {
            new AmountType { Id = 1, Type = "Per Month" },
            new AmountType { Id = 2, Type = "Per Year" },
            new AmountType { Id = 3, Type = "Per Hour" },
            new AmountType { Id = 4, Type = "Per Day" },
            new AmountType { Id = 5, Type = "Fixed Price" }
        };
    }
    #endregion

    public static IEnumerable<User> GetUsers()
    {
        // NOTE: placeholder password hashes - replace with secure hashes when needed
        return new List<User>
        {
            new User { Id = 1, Username = "admin", PasswordHash = "AQAAAAEAACcQAAAAE-admin-hash", Role = "Administrator" },
            new User { Id = 2, Username = "standard.user", PasswordHash = "AQAAAAEAACcQAAAAE-user-hash", Role = "User" }
        };
    }

    #endregion

    public async Task SeedAsync()
    {
        await _seedRepository.SeedEventTypesAsync(GetEventTypes);
        await _seedRepository.SeedEventStatusesAsync(GetEventStates);
        await _seedRepository.SeedDealStatesAsync(GetDealStates);
        await _seedRepository.SeedDealTypesAsync(GetDealTypes);
        await _seedRepository.SeedAmountTypesAsync(GetAmountTypes);
        await _seedRepository.SeedUsersAsync(GetUsers);
        await _seedRepository.SeedAiPromptsAsync(GetAiPrompts);
    }

    public async Task SeedTestDataAsync()
    {
        if (await _seedRepository.AnyTestDataExistsAsync())
        {
            _logger.LogInformation("Test data already exists in the database. Skipping seeding of test data.");
            return;
        }
        // Seed firms first so their IDs are available for deal and contact person assignment
        await _seedRepository.SeedFirmsAsync(GetTestFirms);

        // Seed deals and assign firms to new deals in one pass
        await _seedRepository.SeedDealsAsync(GetTestDeals, AssignFirmToDeal);

        // Seed contact persons per firm (firms must exist first)
        await _seedRepository.SeedFirmContactPersonsAsync(GetTestContactPersonsForFirm);

        await _seedRepository.SeedDealTagsAsync(GetTestDealTags);

        // Seed events after deals/contact persons so foreign keys are valid
        await _seedRepository.SeedEventsAsync(GetTestEventsForDeal);

        await _seedRepository.SeedEventNotesAsync(GetTestEventNotesForEvent);
    }

    /// <summary>
    /// Multiplies deals by cloning existing ones with randomised names/descriptions/URLs until
    /// the Deals table contains at least <paramref name="targetRowCount"/> rows, then fills in
    /// events, event-notes and deal-tags for every deal that does not have them yet.
    /// </summary>
    public async Task MultiplyTestDataAsync(int targetRowCount)
    {
        await _seedRepository.MultiplyFirmsAsync((int)(targetRowCount * 0.7));
        await _seedRepository.MultiplyDealsAsync(targetRowCount);
        await _seedRepository.MultiplyDealDependentsAsync((int)(targetRowCount * 0.7));
    }
    #region AI Stuff seeding

    public static IEnumerable<AiPrompt> GetAiPrompts()
    {
        return new List<AiPrompt>
        {
            ProcessPagePrompt_V1_0
        };
    }

    public static readonly AiPrompt ProcessPagePrompt_V1_0 = new AiPrompt
    {
        Key = "process_page",
        Version = "1.0",
        Text =
@"SYSTEM:
You are an information extraction engine.
Your task is to extract structured job vacancy data from noisy webpage text.

Rules:
- Focus only on the actual job vacancy content
- Do not invent information
- If something is missing, return null
- Be concise but complete
- The 'text' field MUST contain the FULL cleaned job posting content

To extract requirements and nice-to-haves for sections like:
- 'Requirements'
- 'What we are looking for'
- 'Nice to have'
- 'Will be a plus'
- 'Qualifications'

To extract responsibilities for sections like:
- 'responsibilities'
- 'What you will do'
- 'You will be involved into'

Description:
- Extract only the introductory part describing the role/company
- STOP when structured sections begin (requirements/responsibilities/etc.)

CRITICAL FORMATTING REQUIREMENTS:
- The 'text' and 'description' fields must preserve line breaks exactly
- Each logical line or bullet point must remain on its own line
- Never convert multi-line text into a single paragraph
- Output must contain '\n' characters where line breaks exist

USER:
Extract job information from the text below.

CONTEXT:
{{RAW_PAGE_TEXT}}

OUTPUT:
Return ONLY valid JSON with this exact structure:

{
  'text': string | null,
  'title': string | null,
  'company': string | null,
  'location': string | null,
  'description': string,
  'responsibilities': string[],
  'requirements': string[],
  'nice_to_have': string[],
  'perks': string[]
}"
    };
    #endregion
}
