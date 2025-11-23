using FluentValidation;
using Sai.DealAssistant.Application.Commands;

namespace Sai.DealAssistant.Application.Validation
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0m).WithMessage("Price must be greater than or equal to 0.");
        }
    }
}