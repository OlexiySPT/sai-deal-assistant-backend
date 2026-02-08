using FluentValidation;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.General.Commands;

public class UpdateDateFieldCommand : IRequest<DateTimeOffset?>
{
    public string Entity { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public int Id { get; set; }
    public DateTimeOffset? Value { get; set; }
    public bool NotNull { get; set; } = false;

    public class Validator : AbstractValidator<UpdateDateFieldCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            When(c => c.NotNull, () =>
            {
                RuleFor(c => c.Value)
                    .NotNull()
                    .WithMessage("Value must not be null.");
            });
        }
    }

    public class Handler : IRequestHandler<UpdateDateFieldCommand, DateTimeOffset?>
    {
        private readonly IFieldUpdateRepository<DateTimeOffset?> _repository;

        public Handler(IFieldUpdateRepository<DateTimeOffset?> repository)
        {   
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<DateTimeOffset?> Handle(UpdateDateFieldCommand request, CancellationToken cancellationToken)
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