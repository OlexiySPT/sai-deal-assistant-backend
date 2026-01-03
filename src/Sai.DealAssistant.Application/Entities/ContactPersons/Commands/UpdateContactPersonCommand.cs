using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Net.Mail;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Commands;

public class UpdateContactPersonCommand : ContactPersonDto, IRequest<ContactPersonDto>
{
    public class Validator : AbstractValidator<UpdateContactPersonCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

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

    public class Handler : IRequestHandler<UpdateContactPersonCommand, ContactPersonDto>
    {
        private readonly ICrudRepository<ContactPerson> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<ContactPerson> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContactPersonDto> Handle(UpdateContactPersonCommand request, CancellationToken cancellationToken)
        {
            var toUpdate = _mapper.Map<ContactPerson>(request);
            var updated = await _repository.UpdateAsync(toUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(ContactPerson), request.Id);
            }

            return _mapper.Map<ContactPersonDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateContactPersonCommand, ContactPerson>().ReverseMap();
            }
        }
    }
}