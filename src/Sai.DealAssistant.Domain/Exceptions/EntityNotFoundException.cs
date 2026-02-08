namespace Sai.DealAssistant.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string entity, object id)
        : base($"No record found in table '{entity}' with id '{id}'.")
    {
    }
}