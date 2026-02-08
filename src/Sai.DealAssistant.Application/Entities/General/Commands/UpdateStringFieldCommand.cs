using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sai.DealAssistant.Application.Entities.General.Commands;

public enum StringFieldValidationType
{
    None,
    NotNull,
    NotEmpty,
    Email,
    Url
}

public class UpdateStringFieldCommand : IRequest<string>
{

    public string Entity { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public int Id { get; set; }
    public string? Value { get; set; } = string.Empty;
    public StringFieldValidationType Validation { get; set; } = StringFieldValidationType.None;


    public class Validator : AbstractValidator<UpdateStringFieldCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.Value)
                .Custom(
                (value, context) =>
                    {
                        var command = context.InstanceToValidate;
                        switch (command.Validation)
                        {
                            case StringFieldValidationType.NotNull:
                                if (value is null)
                                {
                                    context.AddFailure("Value", "Value must not be null.");
                                }
                                break;
                            case StringFieldValidationType.NotEmpty:
                                if (string.IsNullOrWhiteSpace(value))
                                {
                                    context.AddFailure("Value", "Value must not be empty.");
                                }
                                break;
                            case StringFieldValidationType.Email:
                                if (value != null && !new EmailAddressAttribute().IsValid(value))
                                {
                                    context.AddFailure("Value", "Value must be a valid email address.");
                                }
                                break;
                            case StringFieldValidationType.Url:
                                if (value != null && !Uri.IsWellFormedUriString(value, UriKind.Absolute))
                                {
                                    context.AddFailure("Value", "Value must be a valid URL.");
                                }
                                break;
                        }
                    }
                );
        }
    }

    public class Handler : IRequestHandler<UpdateStringFieldCommand, string?>
    {
        private readonly IFieldUpdateRepository<string> _repository;

        public Handler(IFieldUpdateRepository<string> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<string?> Handle(UpdateStringFieldCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.UpdateFieldAsync(request.Entity, request.Field, request.Id, request.Value);
            }
            catch (EntityNotFoundException ex)
            {
                throw new NotFoundExceptionOverride(request.Entity, request.Id);
            }
            return request.Value;
        }
    }
}