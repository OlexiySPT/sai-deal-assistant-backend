namespace Sai.DealAssistant.Application.Common.Exceptions;

public class AuthorizationExceptionOverride : Exception
{
	public AuthorizationExceptionOverride(string? message)
		: base(message)
	{
	}
}
