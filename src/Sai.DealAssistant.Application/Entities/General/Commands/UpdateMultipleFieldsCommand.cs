using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.General.Commands;

public class FieldUpdate
{
    public string Field { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class UpdateMultipleFieldsCommand : IRequest<IReadOnlyDictionary<string, object?>>
{
    public string Entity { get; set; } = string.Empty;
    public int Id { get; set; }
    public List<FieldUpdate> Fields { get; set; } = [];

    public class Validator : AbstractValidator<UpdateMultipleFieldsCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Entity)
                .NotEmpty()
                .WithMessage("Entity must not be empty.");

            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(c => c.Fields)
                .NotEmpty()
                .WithMessage("At least one field must be provided.");

            RuleForEach(c => c.Fields)
                .ChildRules(field =>
                {
                    field.RuleFor(f => f.Field)
                        .NotEmpty()
                        .WithMessage("Field name must not be empty.");
                });
        }
    }

    public class Handler : IRequestHandler<UpdateMultipleFieldsCommand, IReadOnlyDictionary<string, object?>>
    {
        private readonly IMultiFieldUpdateRepository _repository;

        public Handler(IMultiFieldUpdateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyDictionary<string, object?>> Handle(UpdateMultipleFieldsCommand request, CancellationToken cancellationToken)
        {
            var fieldDict = request.Fields
                .ToDictionary(f => f.Field, f => f.Value);

            try
            {
                await _repository.UpdateFieldsAsync(request.Entity, request.Id, fieldDict);
            }
            catch (EntityNotFoundException)
            {
                throw new NotFoundExceptionOverride(request.Entity, request.Id);
            }

            return fieldDict;
        }
    }
}
