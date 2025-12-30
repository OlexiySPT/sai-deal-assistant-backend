using FluentValidation.Results;

namespace Sai.DealAssistant.Application.Common.Exceptions;

public class ValidationExceptionOverride : Exception
{
	public ValidationExceptionOverride()
		: base("One or more validation failures have occurred.")
	{
		Failures = new Dictionary<string, string[]>();
	}

	public ValidationExceptionOverride(List<ValidationFailure> failures)
		: this()
	{
		IEnumerable<string> propertyNames = failures
			.Select(e => e.PropertyName)
			.Distinct();

		foreach (string propertyName in propertyNames)
		{
			string[] propertyFailures = failures
				.Where(e => e.PropertyName == propertyName)
				.Select(e => e.ErrorMessage)
				.ToArray();

			Failures.Add(propertyName, propertyFailures);
		}
	}

	public IDictionary<string, string[]> Failures { get; }
}
