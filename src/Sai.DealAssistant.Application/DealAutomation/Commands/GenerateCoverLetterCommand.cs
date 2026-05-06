using FluentValidation;
using MediatR;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Domain.AI;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Text.Json.Nodes;

namespace Sai.DealAssistant.Application.DealAutomation.Commands;

public class GenerateCoverLetterCommand : IRequest<string>, IJobQueueCommand
{
    public int DealId { get; set; }

    public class Validator : AbstractValidator<GenerateCoverLetterCommand>
    {
        private readonly ICrudRepository<Deal> _dealRepository;

        public Validator(ICrudRepository<Deal> dealRepository)
        {
            _dealRepository = dealRepository;

            RuleFor(x => x.DealId)
                .GreaterThan(0).WithMessage("DealId must be greater than zero.")
                .MustAsync(DealExistsWithAiFullStructuredInfo)
                .WithMessage("Deal not found or contains empty AiFullStructuredInfo.");
        }

        private async Task<bool> DealExistsWithAiFullStructuredInfo(int dealId, CancellationToken cancellationToken)
        {
            var deal = await _dealRepository.FirstOrDefaultAsync(d => d.Id == dealId);
            if (deal is null)
                return false;

            return !string.IsNullOrWhiteSpace(deal.AiFullStructuredInfo);
        }
    }

    public class Handler : IRequestHandler<GenerateCoverLetterCommand, string>
    {
        private readonly IAiClient _aiClient;
        private readonly ICrudRepository<Deal> _dealRepository;
        IAiPromptRepository _aiPromptRepository;
        IAiMetadataRepository _aiMetadataRepository;

        public Handler(
            IAiClient aiClient,
            ICrudRepository<Deal> dealRepository,
            IAiPromptRepository aiPromptRepository,
            IAiMetadataRepository aiMetadataRepository)
        {
            _aiClient = aiClient;
            _dealRepository = dealRepository;
            _aiPromptRepository = aiPromptRepository;
            _aiMetadataRepository = aiMetadataRepository;
        }

        public async Task<string> Handle(GenerateCoverLetterCommand request, CancellationToken cancellationToken)
        {
            var deal = await _dealRepository.FirstOrDefaultAsync(d => d.Id == request.DealId);
            if (deal is null) throw new ArgumentException($"Deal with id {request.DealId} not found.");
            if(string.IsNullOrEmpty(deal.AiFullStructuredInfo)) throw new ArgumentException($"Deal with id {request.DealId} has empty AiFullStructuredInfo.");
            var aiFullStructuredInfo = deal?.AiFullStructuredInfo;

            JsonArray combinedRequirements = [];
            if (!string.IsNullOrWhiteSpace(aiFullStructuredInfo) && JsonNode.Parse(aiFullStructuredInfo) is JsonObject infoJson)
            {
                if (infoJson["requirements"] is JsonArray requirements)
                {
                    foreach (var item in requirements)
                        combinedRequirements.Add(item?.DeepClone());
                }

                if (infoJson["nice_to_have"] is JsonArray niceToHave)
                {
                    foreach (var item in niceToHave)
                        combinedRequirements.Add(item?.DeepClone());
                }
            }
            var jobRequirements = combinedRequirements.ToJsonString();

            var promptFromDb = await _aiPromptRepository.GetTextAsync("generate_letter");
            var profileFromDb = await _aiMetadataRepository.GetTextAsync("Profile", "DotNet");

            var preparedPrompt = promptFromDb!.Replace("{{candidate_profile_json}}", profileFromDb);
            preparedPrompt = preparedPrompt.Replace("{{job_requirements_json}}", jobRequirements);
            var aiResponseText = await _aiClient.Chat(
                AiTaskTypesEnum.Complex,
                preparedPrompt,
                request.DealId
            );
            var resultJson = JsonNode.Parse(aiResponseText);
            var resultContent = resultJson?["choices"]?[0]?["message"]?["content"]?.ToString();
            deal!.InitialLetter = resultContent;
            await _dealRepository.UpdateAsync(deal);

            return resultContent!;
        }
    }
}