namespace Sai.DealAssistant.Domain.Repositories.Generic
{
    public interface IMultiFieldUpdateRepository
    {
        bool TableExists(string entity);
        bool ColumnExists(string entity, string field);
        Task UpdateFieldsAsync(string entity, int id, IReadOnlyDictionary<string, object?> fields);
    }
}
