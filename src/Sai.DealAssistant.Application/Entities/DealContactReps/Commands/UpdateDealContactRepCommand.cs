using AutoMapper;
using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Net.Mail;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Commands;

public class UpdateDealContactRepCommand : DealContactRepDto, IRequest<DealContactRepDto>
{
    public class Validator : AbstractValidator<UpdateDealContactRepCommand>
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

    public class Handler : IRequestHandler<UpdateDealContactRepCommand, DealContactRepDto>
    {
        private readonly ICrudRepository<DealContactRep> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<DealContactRep> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealContactRepDto> Handle(UpdateDealContactRepCommand request, CancellationToken cancellationToken)
        {
            var toUpdate = _mapper.Map<DealContactRep>(request);
            var updated = await _repository.UpdateAsync(toUpdate);

            if (updated == null)
            {
                throw new NotFoundExceptionOverride(nameof(DealContactRep), request.Id);
            }

            return _mapper.Map<DealContactRepDto>(updated);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateDealContactRepCommand, DealContactRep>().ReverseMap();
            }
        }
    }
}