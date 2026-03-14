namespace Sai.DealAssistant.Application.Common.Exceptions;

public class NotFoundExceptionOverride : Exception
{
	public NotFoundExceptionOverride(string name, object? id)
		: base($"Entity '{name}' with Id={id} was not found.")
	{
    }
    public NotFoundExceptionOverride(string text)
        : base(text)
    {
    }
}
