using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic
{
    public class MultiFieldUpdateRepository<TDbContext> : IMultiFieldUpdateRepository
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public MultiFieldUpdateRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool TableExists(string entity)
        {
            return _dbContext.Model.GetEntityTypes()
                .Any(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
        }

        public bool ColumnExists(string entity, string field)
        {
            var entityType = _dbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
            if (entityType == null)
                return false;

            var property = entityType.ClrType.GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return property != null;
        }

        public async Task UpdateFieldsAsync(string entity, int id, IReadOnlyDictionary<string, object?> fields)
        {
            var entityType = _dbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
            if (entityType == null)
                throw new TableNotExistsException(entity);

            var keyProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (keyProperty == null)
                throw new InvalidOperationException($"No primary key defined for entity '{entity}'.");

            var dbSet = (IQueryable<object>)_dbContext.GetType()
                .GetMethod("Set", Type.EmptyTypes)!
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(_dbContext, null)!;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var propertyAccess = Expression.Property(parameter, keyProperty.Name);
            var idValue = Expression.Constant(Convert.ChangeType(id, propertyAccess.Type));
            var equals = Expression.Equal(propertyAccess, idValue);
            var lambda = Expression.Lambda(equals, parameter);

            var whereMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType.ClrType);
            var query = whereMethod.Invoke(null, new object[] { dbSet, lambda });

            var firstOrDefaultAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType.ClrType);
            var entityObjTask = (Task)firstOrDefaultAsyncMethod.Invoke(null, new object[] { query, default(CancellationToken) })!;
            await entityObjTask.ConfigureAwait(false);
            var entityObj = entityObjTask.GetType().GetProperty("Result")?.GetValue(entityObjTask);

            if (entityObj == null)
                throw new EntityNotFoundException($"No record found in table '{entity}' with id '{id}'.");

            foreach (var (field, newValue) in fields)
            {
                var property = entityType.ClrType.GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                    throw new ColumnNotExistsException(field);

                var targetType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                object? rawValue = newValue is JsonElement jsonElement
                    ? DeserializeJsonElement(jsonElement, underlyingType)
                    : newValue;

                object? convertedValue = rawValue;
                if (rawValue != null && rawValue.GetType() != targetType)
                {
                    convertedValue = Convert.ChangeType(rawValue, underlyingType);
                }
                property.SetValue(entityObj, convertedValue);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static object? DeserializeJsonElement(JsonElement element, Type targetType)
        {
            if (element.ValueKind == JsonValueKind.Null)
                return null;

            if (targetType == typeof(string))
                return element.GetString();

            if (targetType == typeof(bool))
                return element.GetBoolean();

            if (targetType == typeof(Guid))
                return element.GetGuid();

            if (targetType == typeof(DateOnly))
                return DateOnly.Parse(element.GetString()!);

            if (targetType == typeof(DateTimeOffset))
                return element.GetDateTimeOffset();

            if (targetType == typeof(DateTime))
                return element.GetDateTime();

            if (targetType == typeof(int))
                return element.GetInt32();

            if (targetType == typeof(long))
                return element.GetInt64();

            if (targetType == typeof(decimal))
                return element.GetDecimal();

            if (targetType == typeof(double))
                return element.GetDouble();

            if (targetType == typeof(float))
                return element.GetSingle();

            return JsonSerializer.Deserialize(element.GetRawText(), targetType);
        }
    }
}
