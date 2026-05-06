using FluentValidation;
using MediatR;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Domain.AI;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Http;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Application.DealAutomation.Commands;

public class ExtractTextCommand : IRequest<string>, IJobQueueCommand
{
    public string Url { get; set; }
    public int? DealId { get; set; }

    public class Validator : AbstractValidator<ExtractTextCommand>
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

    public class Handler : IRequestHandler<ExtractTextCommand, string>
    {
        private readonly IExternalPageScrapper _scrapper;
        private readonly IAiClient _aiClient;
        IAiPromptRepository _aiPromptRepository;

        public Handler(
            IExternalPageScrapper scrapper,
            IAiClient aiClient,
            IAiPromptRepository aiPromptRepository)
        {
            _scrapper = scrapper;
            _aiClient = aiClient;
            _aiPromptRepository = aiPromptRepository;
        }

        public async Task<string> Handle(ExtractTextCommand request, CancellationToken cancellationToken)
        {
            var scrappedPage = await _scrapper.ReadPageContent(request.Url);
            var promptFromDb = await _aiPromptRepository.GetTextAsync("extract_description_from_page");
            var aiResponseText = await _aiClient.Chat(
                AiTaskTypesEnum.Fast,
                (promptFromDb ?? PROMPT).Replace("{{RAW_PAGE_TEXT}}", scrappedPage),
                request.DealId
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

            return content!;
        }
    }
    #region Prompt
    const string PROMPT =
@"SYSTEM:
You are an information extraction engine.
Your task is to extract the FULL cleaned job posting content from noisy webpage text.

Rules:
- Focus only on the actual job vacancy content
- Do not invent information
- Extract everyth

USER:
Extract job information from the text below.

CONTEXT:
{{RAW_PAGE_TEXT}}

OUTPUT:
Return job posting content as raw text. Maintain line breaks exactly.";
    #endregion

}