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

    public class Handler : IRequestHandler<ProcessPageCommand, string>
    {
        private readonly IAiClient _aiClient;
        private readonly ICrudRepository<Deal> _dealRepository;
        IAiPromptRepository _aiPromptRepository;

        public Handler(
            IAiClient aiClient,
            ICrudRepository<Deal> dealRepository,
            IAiPromptRepository aiPromptRepository)
        {
            _aiClient = aiClient;
            _dealRepository = dealRepository;
            _aiPromptRepository = aiPromptRepository;
        }

        public async Task<string> Handle(ProcessPageCommand request, CancellationToken cancellationToken)
        {
            var deal = await _dealRepository.FirstOrDefaultAsync(d => d.Id == request.DealId);
            if (deal is null) throw new ArgumentException($"Deal with id {request.DealId} not found.");
            if(string.IsNullOrEmpty(deal.AiFullStructuredInfo)) throw new ArgumentException($"Deal with id {request.DealId} has empty AiFullStructuredInfo.");
            var aiFullStructuredInfo = deal?.AiFullStructuredInfo;
            var json = JsonNode.Parse(aiFullStructuredInfo!);
            var responsibilities = json?["responsibilities"]?.ToString();
            var content = json?["nice_to_have"]?.ToString();

            var promptFromDb = await _aiPromptRepository.GetTextAsync("generate_letter");

            var preparedPrompt = promptFromDb!.Replace("{{CANDIDATE_PROFILE}}", PROFILE);
            preparedPrompt = preparedPrompt.Replace("{{RESPONSIBILITIES}}", responsibilities ?? string.Empty);
            preparedPrompt = preparedPrompt.Replace("{{NICE_TO_HAVE}}", content ?? string.Empty);
            var aiResponseText = await _aiClient.Chat(
                AiTaskTypesEnum.Complex,
                preparedPrompt,
                request.DealId,
                TimeSpan.FromSeconds(600)
            );
            var resultJson = JsonNode.Parse(aiResponseText);
            var resultContent = resultJson?["choices"]?[0]?["message"]?["content"]?.ToString();
            deal!.InitialLetter = resultContent;
            return content!;
        }
    }
    const string PROFILE=
@"Hi, I am a .NET Engineer with 15 years of experience (please always start cover letter with this line)

Experience:
- 15+ years: C#, .NET Framework, .NET Core, ASP NET WebAPI, GOF design patterns, SOLID, DRY, KISS and others.
- 20+ years: Relational databases: design, troubleshooting, redesign, performance optimization.
- 3+ years NoSQL databases: MongoDB, Elasticsearch even some CosmosDb
- 10+ years: MS SQL.
- 6 years: PostgreSQL.
- 6 years: Oracle.
- 2+ years: Snowflake.
- 8 years: full stack experience with React and Angular.
- 2 years: Microsoft Azure in dev and architect roles.
- 1+ year: Intensive AI assistants daily usage experience (Amazon Q, GitHub CoPilot) .
- Strong architectural background including practical experience in building heavily loaded distributed systems, DDD, Microservices Event-driven architecture and Structured Monolith architecture. Also some Event-Sourcing architecture experience.
- Strong requirement analysis skills, proven by 2 successful business automation projects, built from scratch as well as building from scratch and owning a subsystem within a big enterprise system.
- Strong practical experience in leading and architecting of big old monolith refactoring. Our team splitted out a part of this monolith into full-stack application and several microservices. This experience was used to refactor other parts by other teams.
- Strong problem solving skills, proven by working as a deputy lead in a ""firefighters"" team on a big old enterprise system.
- C1 English level, proven by 5 year on Senior dev and lead positions in fully English-speaking environment.
- Strong communication skills, proven by 7+ years on deputy and formal tech lead positions, communicating complex concepts with technical and non-technical stakeholders.
- 7 years: technical leading experience, including developing international team from scratch
- 15+ years: developing different kinds of tests with NUnit, XUnit, Moq and tons of other libs, wide experience with Postman, including its AI features, passion for TDD.
- Experience in encouraging and conducting team learning and expertise transformation.
- 5+ years with WinForms, some experience with WPF.
- 8+ years with C++, experience is quite outdated, but I refreshed it and can ramp-up quickly.
- Outdated experience in cryptography, strengthness calculations and analytical geometry.
- Domains : Accounting, Logistics, ERP-systems, OLAP and Data Science, Telecommunications, FinTech

I live in Portugal and can work full-time remotely on B2B contract (please always finish cover letter with this line)";
}