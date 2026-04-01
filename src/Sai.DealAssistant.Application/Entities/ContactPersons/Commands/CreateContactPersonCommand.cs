using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Net.Mail;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Commands;

public class CreateContactPersonCommand : ContactPersonDto, IRequest<ContactPersonDto>
{
    public int FirmId { get; set; }

    public class Validator : AbstractValidator<CreateContactPersonCommand>
    {
        private readonly IReadRepository<Firm> _firmRepository;

        public Validator(IReadRepository<Firm> firmRepository)
        {
            _firmRepository = firmRepository;

            RuleFor(c => c.FirmId)
                .GreaterThan(0)
                .WithMessage("FirmId must be greater than 0.")
                .MustAsync(async (cmd, firmId, cToken) => await _firmRepository.FirstOrDefaultAsync(f => f.Id == firmId) != null)
                .WithMessage(cmd => $"Firm with Id {cmd.FirmId} was not found.");

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

    public class Handler : IRequestHandler<CreateContactPersonCommand, ContactPersonDto>
    {
        private readonly ICrudRepository<ContactPerson> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<ContactPerson> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactPersonDto> Handle(CreateContactPersonCommand request, CancellationToken cancellationToken)
        {
            var newEntity = _mapper.Map<ContactPerson>(request);
            ContactPerson? created = await _repository.CreateAsync(newEntity);

            if (created == null)
            {
                throw new NotFoundExceptionOverride(nameof(ContactPerson), request.FirmId);
            }

            return _mapper.Map<ContactPersonDto>(created);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateContactPersonCommand, ContactPerson>();
            }
        }
    }
}