using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Net.Mail;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Commands;

public class CreateDealContactRepCommand : DealContactRepDto, IRequest<DealContactRepDto>
{
    public int DealId { get; set; }

    public class Validator : AbstractValidator<CreateDealContactRepCommand>
    {
        private readonly IReadRepository<Deal> _dealRepository;

        public Validator(IReadRepository<Deal> dealRepository)
        {
            _dealRepository = dealRepository;

            RuleFor(c => c.DealId)
                .GreaterThan(0)
                .WithMessage("DealId must be greater than 0.")
                .MustAsync(async (cmd, dealId, cToken) => await _dealRepository.FirstOrDefaultAsync(d => d.Id == dealId) != null)
                .WithMessage(cmd => $"Deal with Id {cmd.DealId} was not found.");

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Name must be provided.");

            RuleFor(c => c.Email)
                .MaximumLength(150)
                .WithMessage("Email must be at most 150 characters.")
                .Must(email => string.IsNullOrWhiteSpace(email) || IsValidEmail(email))
                .WithMessage("Email must be a valid email address.");
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class Handler : IRequestHandler<CreateDealContactRepCommand, DealContactRepDto>
    {
        private readonly ICrudRepository<DealContactRep> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<DealContactRep> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealContactRepDto> Handle(CreateDealContactRepCommand request, CancellationToken cancellationToken)
        {
            var newEntity = _mapper.Map<DealContactRep>(request);
            DealContactRep? created = await _repository.CreateAsync(newEntity);

            if (created == null)
            {
                throw new NotFoundExceptionOverride(nameof(DealContactRep), request.DealId);
            }

            return _mapper.Map<DealContactRepDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateDealContactRepCommand, DealContactRep>();
            }
        }
    }
}