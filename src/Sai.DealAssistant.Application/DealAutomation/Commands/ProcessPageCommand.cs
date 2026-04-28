using FluentValidation;
using MediatR;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Domain.AI;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Http;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.DealAutomation.Commands;

public class ProcessPageCommand : IRequest<string>, IJobQueueCommand
{
    public string Url { get; set; }
    public int? DealId { get; set; }

    public class Validator : AbstractValidator<ProcessPageCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("URL must be provided.")
                .Must(BeAValidUrl).WithMessage("URL must be a valid absolute URL.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class Handler : IRequestHandler<ProcessPageCommand, string>
    {
        private readonly IExternalPageScrapper _scrapper;
        private readonly IAiClient _aiClient;
        private readonly ICrudRepository<Deal> _dealRepository;
        IAiPromptRepository _aiPromptRepository;

        public Handler(
            IExternalPageScrapper scrapper,
            IAiClient aiClient,
            ICrudRepository<Deal> dealRepository,
            IAiPromptRepository aiPromptRepository)
        {
            _scrapper = scrapper;
            _aiClient = aiClient;
            _dealRepository = dealRepository;
            _aiPromptRepository = aiPromptRepository;
        }

        public async Task<string> Handle(ProcessPageCommand request, CancellationToken cancellationToken)
        {
            var scrappedPage = await _scrapper.ReadPageContent(request.Url);
            var promptFromDb = await _aiPromptRepository.GetTextAsync("process_page");
            var aiResponseText = await _aiClient.Chat(
                AiTaskTypesEnum.Fast,
                (promptFromDb ?? PROMPT).Replace("{{RAW_PAGE_TEXT}}", scrappedPage),
                request.DealId,
                TimeSpan.FromSeconds(600)
            );
            var json = JsonNode.Parse(aiResponseText);
            var content = json?["choices"]?[0]?["message"]?["content"]?.ToString();
            // Extract the first JSON object from the content string
            if (!string.IsNullOrWhiteSpace(content))
            {
                var match = Regex.Match(content, "{[\\s\\S]*}");
                if (match.Success)
                    content = match.Value;
            }

            return content;
        }
    }
    #region Prompt
    const string PROMPT =
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
}";
    #endregion

}