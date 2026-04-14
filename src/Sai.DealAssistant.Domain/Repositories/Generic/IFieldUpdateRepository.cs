namespace Sai.DealAssistant.Domain.Repositories.Generic
{
    public interface IFieldUpdateRepository<TFieldType>
    {
        bool TableExists(string entity);
        bool ColumnExists(string entity, string field);
        Task UpdateFieldAsync(string entity, string field, int id, TFieldType? newValue);
    }
}