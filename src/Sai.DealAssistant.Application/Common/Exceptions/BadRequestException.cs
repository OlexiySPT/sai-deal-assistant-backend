namespace Sai.DealAssistant.Application.Common.Exceptions;

public class BadRequestExceptionOverride : Exception
{
	public BadRequestExceptionOverride(string message)
		: base(message)
	{
	}
}
