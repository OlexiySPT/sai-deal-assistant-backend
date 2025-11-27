using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Sai.DealAssistant.Application.Common.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(
		TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

		List<ValidationFailure> failures = new List<ValidationFailure>();

		foreach (IValidator<TRequest> validator in _validators)
		{
			ValidationResult result = await validator.ValidateAsync(context, cancellationToken);

			if (result.Errors.Any())
			{
				failures.AddRange(result.Errors);
			}
		}

		if (failures.Count != 0)
		{
			throw new ValidationException(failures);
		}

		return await next();
	}
}
